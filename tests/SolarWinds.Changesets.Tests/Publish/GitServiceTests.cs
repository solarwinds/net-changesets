using AwesomeAssertions;
using SolarWinds.Changesets.Commands.Publish.Services;
using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Tests.Publish;

[TestFixture]
internal sealed class GitServiceTests
{
    private string _tempRepositoryAbsolutePath = string.Empty;
    private ProcessExecutor _processExecutor = null!;
    private GitService _gitService = null!;

    [SetUp]
    public async Task SetUp()
    {
        string testDirectory = TestContext.CurrentContext.TestDirectory;
        _tempRepositoryAbsolutePath = Path.Join(testDirectory, $"git-service-test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempRepositoryAbsolutePath);

        _processExecutor = new ProcessExecutor();
        await ExecuteGitCommand("init");
        await ExecuteGitCommand("config --local user.email \"test@example.com\"");
        await ExecuteGitCommand("config --local user.name \"Test User\"");
        await ExecuteGitCommand("config --local commit.gpgSign false");

        _gitService = new GitService(_processExecutor);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(_tempRepositoryAbsolutePath))
        {
            DeleteDirectoryWithRetry(_tempRepositoryAbsolutePath);
        }
    }

    [Test]
    public async Task GetDiff_WithModifiedCsprojFile_ReturnsModifiedProject()
    {
        // Arrange
        string projectName = "TestProject";
        string csprojAbsolutePath = Path.Join(_tempRepositoryAbsolutePath, $"{projectName}.csproj");

        // Create initial .csproj file
        await File.WriteAllTextAsync(csprojAbsolutePath, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <VersionPrefix>1.0.0</VersionPrefix>
              </PropertyGroup>
            </Project>
        """
        );

        await ExecuteGitCommand("add .");
        await ExecuteGitCommand("commit -m \"Initial commit\"");

        // Modify .csproj file
        await File.WriteAllTextAsync(csprojAbsolutePath, """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <VersionPrefix>1.1.0</VersionPrefix>
              </PropertyGroup>
            </Project>
        """
        );

        await ExecuteGitCommand("add .");
        await ExecuteGitCommand("commit -m \"Update version\"");

        // Act
        ProcessOutput result = await _gitService.GetDiff(_tempRepositoryAbsolutePath, string.Empty);

        // Assert
        result.ExitCode.Should().Be(0);
        result.Output.Should().HaveCount(1);
        result.Output.First().Should().EndWith($"{projectName}.csproj");
    }

    [Test]
    public async Task GetDiff_WithNonCsprojChanges_ReturnsEmpty()
    {
        // Arrange
        string readmePath = Path.Join(_tempRepositoryAbsolutePath, "README.md");

        await File.WriteAllTextAsync(readmePath, "# Test");
        await ExecuteGitCommand("add .");
        await ExecuteGitCommand("commit -m \"Initial commit\"");

        await File.WriteAllTextAsync(readmePath, "# Test Updated");
        await ExecuteGitCommand("add .");
        await ExecuteGitCommand("commit -m \"Update README\"");

        // Act
        ProcessOutput result = await _gitService.GetDiff(_tempRepositoryAbsolutePath, string.Empty);

        // Assert
        result.ExitCode.Should().Be(0);
        result.Output.Should().HaveCount(1);
        result.Output.First().Should().EndWith($".md");
    }

    [Test]
    public async Task GetDiff_WithMultipleCsprojFiles_ReturnsAllModified()
    {
        // Arrange
        string project1Path = Path.Join(_tempRepositoryAbsolutePath, "Project1.csproj");
        string project2Path = Path.Join(_tempRepositoryAbsolutePath, "Project2.csproj");

        await File.WriteAllTextAsync(project1Path, "<Project />");
        await File.WriteAllTextAsync(project2Path, "<Project />");
        await ExecuteGitCommand("add .");
        await ExecuteGitCommand("commit -m \"Initial commit\"");

        await File.WriteAllTextAsync(project1Path, "<Project><PropertyGroup /></Project>");
        await File.WriteAllTextAsync(project2Path, "<Project><PropertyGroup /></Project>");
        await ExecuteGitCommand("add .");
        await ExecuteGitCommand("commit -m \"Update projects\"");

        // Act
        ProcessOutput result = await _gitService.GetDiff(_tempRepositoryAbsolutePath, string.Empty);

        // Assert
        result.ExitCode.Should().Be(0);
        result.Output.Should().HaveCount(2);
        result.Output.Any(p => p.EndsWith("Project1.csproj", StringComparison.InvariantCulture)).Should().BeTrue();
        result.Output.Any(p => p.EndsWith("Project2.csproj", StringComparison.InvariantCulture)).Should().BeTrue();
    }

    /// <summary>
    /// Helper method to execute git commands in the test repository using ProcessExecutor.
    /// </summary>
    private async Task ExecuteGitCommand(string arguments)
    {
        ProcessOutput result = await _processExecutor.Execute("git", arguments, _tempRepositoryAbsolutePath);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException($"Git command failed: {arguments}. {result.Output}");
        }
    }

    /// <summary>
    /// Deletes a directory with retry logic to handle file locks from Git processes.
    /// </summary>
    private static void DeleteDirectoryWithRetry(string path, int maxRetries = 3)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Directory.Delete(path, recursive: true);
                return;
            }
            catch (IOException) when (i < maxRetries - 1)
            {
                // Wait for file handles to be released
                Thread.Sleep(100);
            }
            catch (UnauthorizedAccessException) when (i < maxRetries - 1)
            {
                // Remove read-only attributes and retry
                RemoveReadOnlyAttributes(path);
                Thread.Sleep(100);
            }
        }
    }

    /// <summary>
    /// Removes read-only attributes from all files in a directory recursively.
    /// </summary>
    private static void RemoveReadOnlyAttributes(string path)
    {
        DirectoryInfo directory = new(path);

        foreach (FileInfo file in directory.GetFiles("*", SearchOption.AllDirectories))
        {
            file.Attributes &= ~FileAttributes.ReadOnly;
        }
    }
}

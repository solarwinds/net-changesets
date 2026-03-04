using System.Security.Cryptography;
using System.Xml;
using AwesomeAssertions;
using SolarWinds.Changesets.Commands.Version;
using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Version;

[TestFixture]
internal sealed class DotnetProjectsRepositoryTests
{
    private static readonly string s_testProjectWithVersion = Path.Join(Environment.CurrentDirectory, "TestData", "SubDirectory", "TestProjectWithVersion.csproj");
    private static readonly string s_testFsharpProjectWithVersion = Path.Join(Environment.CurrentDirectory, "TestData", "SubDirectory", "TestFsharpProjectWithVersion.fsproj");
    private static readonly string s_testProjectWithoutVersion = Path.Join(Environment.CurrentDirectory, "TestData", "TestProjectWithoutVersion.csproj");
    private static readonly string s_testFilePath = Path.Join(Environment.CurrentDirectory, "TestData", "test.csproj");
    private static readonly string s_testFsharpFilePath = Path.Join(Environment.CurrentDirectory, "TestData", "test.fsproj");

    [TearDown]
    public void TearDown()
    {
        DeleteFile(s_testFilePath);
        DeleteFile(s_testFsharpFilePath);
    }

    [Test]
    public async Task UpdateModuleCsProjsAsync_IncreasesVersion_WhenVersionInProjectFile()
    {
        DeleteFile(s_testFilePath);

        File.Copy(s_testProjectWithVersion, s_testFilePath);

        IEnumerable<ModuleChangelog> changes = [
            new ModuleChangelog()
            {
                ModuleName = "ProjectWithVersion",
                CurrentVersion = new Semver(1, 0, 0),
                ModuleProjectFilePath = s_testFilePath,
                Changes = [("abc", BumpType.Minor)]
            }];
        using TestConsole testConsole = new();
        DotnetProjectsRepository projectsRepository = new(testConsole);

        await projectsRepository.UpdateProjectsVersionAsync(changes);

        Semver? versionFromFile = LoadVersionFromProjectFile(s_testFilePath);

        versionFromFile?.Should().BeEquivalentTo(new Semver(1, 1, 0));
    }

    [Test]
    public async Task UpdateModuleCsProjsAsync_DoesNotTouchProjectFile_WhenNoVersionInProjectFile()
    {
        DeleteFile(s_testFilePath);

        File.Copy(s_testProjectWithoutVersion, s_testFilePath);

        string hashOriginal = ComputeFileHash(s_testFilePath);

        IEnumerable<ModuleChangelog> changes = [
            new ModuleChangelog()
            {
                ModuleName = "ProjectWithoutVersion",
                CurrentVersion = new Semver(0, 0, 0),
                ModuleProjectFilePath = s_testFilePath,
                Changes = [("abc", BumpType.Minor)]
            }];

        using TestConsole testConsole = new();
        DotnetProjectsRepository projectsRepository = new(testConsole);

        await projectsRepository.UpdateProjectsVersionAsync(changes);

        LoadVersionFromProjectFile(s_testFilePath).Should().BeNull();

        ComputeFileHash(s_testFilePath).Should().Be(hashOriginal);
    }

    [Test]
    public void GetCsProjects_OnlyOneProjectWithValidVersion_ReturnsSingleProject()
    {
        ChangesetConfig config = new()
        {
            SourcePath = "TestData"
        };

        using TestConsole testConsole = new();
        DotnetProjectsRepository projectsRepository = new(testConsole);

        DotnetProject[] projects = projectsRepository.GetProjects(config);
        projects.Length.Should().Be(2);

        projects
            .Single(p => p.Name == "TestProjectWithVersion")
            .ReferencedProjectNames
            .Length
            .Should()
            .Be(2)
            ;

        projects
            .Single(p => p.Name == "TestFsharpProjectWithVersion")
            .ReferencedProjectNames
            .Length
            .Should()
            .Be(1)
            ;
    }

    [Test]
    public async Task UpdateFsharpProjectVersion_IncreasesVersion_WhenVersionInProjectFile()
    {
        DeleteFile(s_testFsharpFilePath);

        File.Copy(s_testFsharpProjectWithVersion, s_testFsharpFilePath);

        IEnumerable<ModuleChangelog> changes = [
            new ModuleChangelog()
            {
                ModuleName = "FsharpProjectWithVersion",
                CurrentVersion = new Semver(2, 0, 0),
                ModuleProjectFilePath = s_testFsharpFilePath,
                Changes = [("abc", BumpType.Minor)]
            }];
        using TestConsole testConsole = new();
        DotnetProjectsRepository projectsRepository = new(testConsole);

        await projectsRepository.UpdateProjectsVersionAsync(changes);

        Semver? versionFromFile = LoadVersionFromProjectFile(s_testFsharpFilePath);

        versionFromFile?.Should().BeEquivalentTo(new Semver(2, 1, 0));
    }

    private static void DeleteFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private static Semver? LoadVersionFromProjectFile(string path)
    {
        XmlDocument doc = new();
        doc.Load(path);

        XmlNode? versionNode = doc.DocumentElement?.SelectSingleNode("/Project/PropertyGroup/Version");
        if (versionNode != null && Semver.TryParse(versionNode.InnerText, out Semver? parsedVersion))
        {
            return parsedVersion;
        }

        return null;
    }

    private static string ComputeFileHash(string path)
    {
        using SHA256 sha256 = SHA256.Create();
        using FileStream stream = File.OpenRead(path);

        byte[] hashBytes = sha256.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "", StringComparison.Ordinal).ToUpperInvariant();
    }
}

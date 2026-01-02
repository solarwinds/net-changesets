using System.Security.Cryptography;
using System.Xml;
using AwesomeAssertions;
using SolarWinds.Changesets.Commands.Version;
using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Version;

[TestFixture]
internal sealed class CsProjectsRepositoryTests
{
    private static readonly string s_testProjectWithVersion = Path.Join(Environment.CurrentDirectory, "TestData", "SubDirectory", "TestProjectWithVersion.csproj");
    private static readonly string s_testProjectWithoutVersion = Path.Join(Environment.CurrentDirectory, "TestData", "TestProjectWithoutVersion.csproj");
    private static readonly string s_testFilePath = Path.Join(Environment.CurrentDirectory, "TestData", "test.csproj");

    [TearDown]
    public void TearDown()
    {
        DeleteFile(s_testFilePath);
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
                ModuleCsProjFilePath = s_testFilePath,
                Changes = [("abc", BumpType.Minor)]
            }];
        TestConsole testConsole = new();
        CsProjectsRepository csProjFileHelper = new(testConsole);

        await csProjFileHelper.UpdateCsProjectsVersionAsync(changes);

        Semver? versionFromFile = LoadVersionFromProjectFile(s_testFilePath);

        versionFromFile?.Should().BeEquivalentTo(new Semver(1, 1, 0));
        testConsole.Dispose();
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
                ModuleCsProjFilePath = s_testFilePath,
                Changes = [("abc", BumpType.Minor)]
            }];

        TestConsole testConsole = new();
        CsProjectsRepository csProjFileHelper = new(testConsole);

        await csProjFileHelper.UpdateCsProjectsVersionAsync(changes);

        LoadVersionFromProjectFile(s_testFilePath).Should().BeNull();

        ComputeFileHash(s_testFilePath).Should().Be(hashOriginal);
        testConsole.Dispose();
    }

    [Test]
    public void GetCsProjects_OnlyOneProjectWithValidVersion_ReturnsSingleProject()
    {
        ChangesetConfig config = new()
        {
            SourcePath = "TestData"
        };

        TestConsole testConsole = new();
        CsProjectsRepository csProjFileHelper = new(testConsole);

        CsProject[] csProjects = csProjFileHelper.GetCsProjects(config);
        csProjects.Length.Should().Be(1);

        csProjects
            .Single()
            .ReferencedProjectNames
            .Length
            .Should()
            .Be(2)
            ;

        testConsole.Dispose();
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

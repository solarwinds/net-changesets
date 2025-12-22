using AwesomeAssertions;
using SolarWinds.Changesets.Commands.Version;
using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Tests.Version;

[TestFixture]
internal sealed class ChangelogFileWriterTests
{
    private static readonly string s_changelogFilePath = Path.Join(Environment.CurrentDirectory, Constants.ChangelogFileName);

    [Test]
    public async Task GenerateChangelogFilesAsync_GeneratesNewFile_WhenFileDoesNotExist()
    {
        ChangelogFileWriter fileWriter = new();

        IEnumerable<ModuleChangelog> changes = [
            new ModuleChangelog()
            {
                ModuleName = "Project",
                CurrentVersion = new Semver(1, 0, 0),
                ModuleCsProjFilePath = Path.Join(Environment.CurrentDirectory, "project.csproj"),
                Changes = [("Something has changed", BumpType.Minor)]
            }];

        string expectedContent = """
# Project

## 1.1.0

**Minor Changes**:

- Something has changed

""";

        await fileWriter.GenerateChangelogFilesAsync(changes);

        File.Exists(s_changelogFilePath).Should().BeTrue();

        AssertChangelogContent(expectedContent);
    }

    [Test]
    public async Task GenerateChangelogFilesAsync_AmendsExistingFile_WhenFileExists()
    {
        ChangelogFileWriter fileWriter = new();

        IEnumerable<ModuleChangelog> changes = [
            new ModuleChangelog()
            {
                ModuleName = "Project",
                CurrentVersion = new Semver(1, 0, 0),
                ModuleCsProjFilePath = Path.Join(Environment.CurrentDirectory, "project.csproj"),
                Changes = [
                    ("change1", BumpType.Minor),
                    ("change2", BumpType.Major)
                ]
            }];

        IEnumerable<ModuleChangelog> changes2 = [
            new ModuleChangelog()
            {
                ModuleName = "Project",
                CurrentVersion = new Semver(2, 0, 0),
                ModuleCsProjFilePath = Path.Join(Environment.CurrentDirectory, "project.csproj"),
                Changes = [
                    ("change3", BumpType.Minor),
                    ("change4", BumpType.Major)
                ]
            }];

        string expectedContent = """
# Project

## 3.0.0

**Major Changes**:

- change4

**Minor Changes**:

- change3

## 2.0.0

**Major Changes**:

- change2

**Minor Changes**:

- change1

""";

        await fileWriter.GenerateChangelogFilesAsync(changes);

        File.Exists(s_changelogFilePath).Should().BeTrue();

        await fileWriter.GenerateChangelogFilesAsync(changes2);

        File.Exists(s_changelogFilePath).Should().BeTrue();
        AssertChangelogContent(expectedContent);
    }

    [Test]
    public async Task GenerateChangelogFilesAsync_GeneratesTwoChangelogs_WhenChangelogsForMultipleProjects()
    {
        ChangelogFileWriter fileWriter = new();

        IEnumerable<ModuleChangelog> changes = [
            new ModuleChangelog()
            {
                ModuleName = "Project",
                CurrentVersion = new Semver(1, 0, 0),
                ModuleCsProjFilePath = Path.Join(Environment.CurrentDirectory, "project.csproj"),
                Changes = [
                    ("change1", BumpType.Minor),
                    ("change2", BumpType.Major)
                ]
            },
            new ModuleChangelog()
            {
                ModuleName = "Project1",
                CurrentVersion = new Semver(1, 0, 0),
                ModuleCsProjFilePath = Path.Join(Environment.CurrentDirectory, "TestData", "project1.csproj"),
                Changes = [
                    ("change3", BumpType.Minor),
                    ("change4", BumpType.Major)
                ]
            }];

        string expectedContent = """
# Project

## 2.0.0

**Major Changes**:

- change2

**Minor Changes**:

- change1

""";

        await fileWriter.GenerateChangelogFilesAsync(changes);

        File.Exists(s_changelogFilePath).Should().BeTrue();
        File.Exists(Path.Join(changes.Last().ModuleDirectoryPath, Constants.ChangelogFileName)).Should().BeTrue();
        AssertChangelogContent(expectedContent);
    }

    [TearDown]
    public void TearDown()
    {
        string[] mdFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.md", SearchOption.TopDirectoryOnly);

        if (mdFiles.Length > 0)
        {
            foreach (string file in mdFiles)
            {
                File.Delete(file);
            }
        }
    }

    private static void AssertChangelogContent(string expectedChangelogContent)
    {
        string actualContent = File.ReadAllText(Path.Join(Environment.CurrentDirectory, Constants.ChangelogFileName));

        actualContent.Should().Be(expectedChangelogContent);
    }
}

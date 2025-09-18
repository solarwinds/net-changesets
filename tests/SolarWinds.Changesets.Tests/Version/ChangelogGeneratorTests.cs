using AwesomeAssertions;
using SolarWinds.Changesets.Commands.Version;
using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Tests.Version;

[TestFixture]
internal sealed class ChangelogGeneratorTests
{
    [Test]
    public void GetProcessedChangelogs_OneChangelogOneIrrelevantProject_ReturnsNoChangelog()
    {
        ChangelogGenerator changelogGenerator = new();

        ChangesetFile[] changesetFile = [
            new(
                ["project1"],
                BumpType.Minor,
                "Changelog for single project")
            ];

        IEnumerable<CsProject> csProject = [
            new("projectX", new Semver(1, 0, 0), ["ProjectReferenceX"], @"c:\temp")
            ];

        List<ModuleChangelog> processedChangelogs = changelogGenerator.GetProcessedChangelogs(changesetFile, csProject).ToList();

        processedChangelogs.Count.Should().Be(0);
    }

    [Test]
    public void GetProcessedChangelogs_OneChangelogTwoProjects_ReturnsOneChangelog()
    {
        ChangelogGenerator changelogGenerator = new();

        ChangesetFile[] changesetFile = [
            new(
                ["project1"],
                BumpType.Minor,
                "Changelog for single project")
            ];

        IEnumerable<CsProject> csProjects = [
            new("project1", new Semver(1, 0, 0), ["ProjectReferenceX"], @"c:\temp"),
            new("project2", new Semver(1, 1, 0), ["ProjectReferenceY"], @"c:\temp")
            ];

        List<ModuleChangelog> processedChangelogs = changelogGenerator.GetProcessedChangelogs(changesetFile, csProjects).ToList();

        processedChangelogs.Count.Should().Be(1);

        ModuleChangelog changelog = processedChangelogs.First();

        changelog.Changes.Count.Should().Be(1);
        changelog.GetNewVersion().Should().BeEquivalentTo(new Semver(1, 1, 0));
        changelog.ModuleName.Should().Be("project1");
    }

    [Test]
    public void GetProcessedChangelogs_OneChangelogThreeProjectsOneProjectReference_ReturnsTwoChangelogs()
    {
        ChangelogGenerator changelogGenerator = new();

        ChangesetFile[] changesetFile = [
            new(
                ["project1"],
                BumpType.Minor,
                "Changelog for single project")
            ];

        IEnumerable<CsProject> csProjects = [
            new("project1", new Semver(1, 0, 0), ["ProjectReferenceX"], @"c:\temp"),
            new("project2", new Semver(1, 1, 0), ["ProjectReferenceY"], @"c:\temp"),
            new("project3", new Semver(1, 1, 1), ["project1"], @"c:\temp")
            ];

        List<ModuleChangelog> processedChangelogs = changelogGenerator.GetProcessedChangelogs(changesetFile, csProjects).ToList();

        processedChangelogs.Count.Should().Be(2);

        ModuleChangelog firstModule = processedChangelogs.First(x => x.ModuleName == "project1");
        ModuleChangelog secondModule = processedChangelogs.First(x => x.ModuleName == "project3");

        firstModule.Changes.Should().HaveCount(1);
        firstModule.GetNewVersion().Should().BeEquivalentTo(new Semver(1, 1, 0));
        secondModule.Changes.Should().HaveCount(1);
        secondModule.GetNewVersion().Should().BeEquivalentTo(new Semver(1, 1, 2));
    }

    [Test]
    public void GetProcessedChangelogs_OneProject_ReturnsOneChangelog()
    {
        ChangelogGenerator changelogGenerator = new();
        ChangesetFile[] changesetFiles = [
            new(
                ["project1"],
                BumpType.Minor,
                "Changelog for single project"),
            new(
                ["project1"],
                BumpType.Patch,
                "Second changelog for project")
            ];
        IEnumerable<CsProject> csProject = [
            new("project1", new Semver(1, 0, 0), ["ProjectReferenceX"], @"c:\path")];

        List<ModuleChangelog> processedChangelogs = changelogGenerator.GetProcessedChangelogs(changesetFiles, csProject).ToList();

        processedChangelogs.Count.Should().Be(1);

        ModuleChangelog changeset = processedChangelogs.First();

        changeset.Changes.Count.Should().Be(2);
        changeset.GetNewVersion().Should().BeEquivalentTo(new Semver(1, 1, 0));
    }

    [Test]
    public void GetProcessedChangelogs_OneProjectTwoPatches_UpdatesSemVerByOne()
    {
        ChangelogGenerator changelogGenerator = new();
        ChangesetFile[] changesetFiles = [
            new(
                ["project1"],
                BumpType.Patch,
                "Changelog for single project"),
            new(
                ["project1"],
                BumpType.Patch,
                "Second changelog for project")
            ];
        IEnumerable<CsProject> csProject = [
            new("project1", new Semver(1, 0, 0), ["ProjectReferenceX"], @"c:\path")];

        List<ModuleChangelog> processedChangelogs = changelogGenerator.GetProcessedChangelogs(changesetFiles, csProject).ToList();

        processedChangelogs.Count.Should().Be(1);

        ModuleChangelog changelog = processedChangelogs.First();

        changelog.Changes.Count.Should().Be(2);
        changelog.GetNewVersion().Should().BeEquivalentTo(new Semver(1, 0, 1));
    }
}

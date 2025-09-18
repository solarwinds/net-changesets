using System.Xml;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using SolarWinds.Changesets.Commands.Add;
using SolarWinds.Changesets.Commands.Version;
using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Infrastructure;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Version;

[TestFixture]
internal sealed class VersionChangesetCommandTests
{
    private const string ChangesetFolder = ".changeset";
    private const string SrcFolder = "src";
    private const string ChangesetFile = "usarjcqmfu.md";
    private const string ChangelogFile = "CHANGELOG.md";

    [SetUp]
    public void SetUp()
    {
        string testDataDirectoryFullPath = Path.Combine(Environment.CurrentDirectory, "Version", "TestData");
        CopyDirectory(testDataDirectoryFullPath, Environment.CurrentDirectory, true, true);
    }

    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(SrcFolder))
        {
            Directory.Delete(SrcFolder, true);
        }

        if (Directory.Exists(ChangesetFolder))
        {
            Directory.Delete(ChangesetFolder, true);
        }
    }

    [Test]
    public void VersionCommand_HappyPath_FolderAndFilesAreCreated()
    {
        ServiceCollection serviceCollection = new();

        serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
        serviceCollection.AddSingleton<IProjectFileNamesLocator, CsharpProjectFileNamesLocator>();
        serviceCollection.AddSingleton<IChangesetsRepository, ChangesetsRepository>();
        serviceCollection.AddSingleton<IChangelogFileWriter, ChangelogFileWriter>();
        serviceCollection.AddSingleton<IChangelogGenerator, ChangelogGenerator>();
        serviceCollection.AddSingleton<ICsProjectsRepository, CsProjectsRepository>();

        TypeRegistrar typeRegistrar = new(serviceCollection);
        var app = new CommandAppTester(typeRegistrar);
        app.SetDefaultCommand<VersionChangesetCommand>();

        CommandAppResult result = app.Run();

        AssertVersionOutput();
    }

    private static void AssertVersionOutput()
    {
        //changesetfile is deleted
        File.Exists(Path.Combine(Environment.CurrentDirectory, ChangesetFolder, ChangesetFile)).Should().BeFalse();

        //project files are updated
        LoadVersionFromProjectFile(Path.Combine(Environment.CurrentDirectory, SrcFolder, "ProjectA", "ProjectA.csproj")).Should().BeEquivalentTo(new Semver(1, 0, 2));
        LoadVersionFromProjectFile(Path.Combine(Environment.CurrentDirectory, SrcFolder, "ProjectB", "ProjectB.csproj")).Should().BeEquivalentTo(new Semver(1, 1, 0));

        //changelogs are created
        File.Exists(Path.Combine(Environment.CurrentDirectory, SrcFolder, "ProjectA", ChangelogFile)).Should().BeTrue();
        File.Exists(Path.Combine(Environment.CurrentDirectory, SrcFolder, "ProjectB", ChangelogFile)).Should().BeTrue();
    }

    private static Semver? LoadVersionFromProjectFile(string path)
    {
        XmlDocument doc = new();
        doc.Load(path);

        XmlNode? versionNode = doc.DocumentElement?.SelectSingleNode("/Project/PropertyGroup/Version");

        return versionNode != null ? Semver.FromString(versionNode.InnerText) : null;
    }

    private static void CopyDirectory(string sourceDirectory, string destinationDirectory, bool recursive, bool firstRun)
    {
        DirectoryInfo directoryInfo = new(sourceDirectory);

        if (!directoryInfo.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory not found: '{directoryInfo.FullName}'.");
        }

        DirectoryInfo[] dirs = directoryInfo.GetDirectories();

        if (!firstRun)
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        foreach (FileInfo file in directoryInfo.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDirectory, file.Name);
            file.CopyTo(targetFilePath);
        }

        if (recursive)
        {
            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDirectory, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, true, false);
            }
        }
    }
}

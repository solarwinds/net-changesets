using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using SolarWinds.Changesets.Commands.Init;
using SolarWinds.Changesets.Infrastructure;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Init;

[TestFixture]
internal sealed class InitCommandTests
{
    private CommandAppTester _app = null!;

    [SetUp]
    public void SetUp()
    {
        CleanUp();

        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
        TypeRegistrar typeRegistrar = new(serviceCollection);
        CommandAppTester app = new(typeRegistrar);
        app.SetDefaultCommand<InitChangesetCommand>();
        _app = app;
    }

    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

    [Test]
    public void InitCommand_HappyPath_FolderAndFilesAreCreated()
    {
        // Arrange, Act
        CommandAppResult result = _app.Run();

        // Assert
        AssertInitCommand(result, ResultCodes.Success);
    }

    [Test]
    public void InitCommand_HappyPathRunTwice_ConfigIsMissingAndWillBeGenerated()
    {
        // Arrange, Act, Assert
        CommandAppResult result = _app.Run();
        AssertInitCommand(result, ResultCodes.Success);

        File.Delete(Constants.ChangesetConfigFileFullPath);

        CommandAppResult result2 = _app.Run();
        AssertInitCommand(result2, ResultCodes.ConfigFileWasGenerated);
    }

    [Test]
    public void InitCommand_HappyPathRunTwice_ConsoleContainMessageThatAlreadyExists()
    {
        // Arrange, Act, Assert
        CommandAppResult result = _app.Run();
        AssertInitCommand(result, ResultCodes.Success);

        CommandAppResult result2 = _app.Run();
        AssertInitCommand(result2, ResultCodes.AlreadyInitialized);
    }

    private static void AssertInitCommand(CommandAppResult result, int expectedExitCode)
    {
        result.ExitCode.Should().Be(expectedExitCode, because: result.Output);
        result.Output.Should().NotBeNullOrEmpty();

        string expectedFolderPath = Constants.ChangesetDirectoryFullPath;
        Directory.Exists(expectedFolderPath).Should().BeTrue($"because the '{Constants.ChangesetDirectoryName}' folder should be created by the command");

        File.Exists(Constants.ChangesetConfigFileFullPath).Should().BeTrue("because the 'config.json' file should be created by the command");
    }

    private static void CleanUp()
    {
        if (Directory.Exists(Constants.ChangesetDirectoryName))
        {
            Directory.Delete(Constants.ChangesetDirectoryName, true);
        }
    }
}

using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWinds.Changesets.Commands.Add;
using SolarWinds.Changesets.Commands.Init;
using SolarWinds.Changesets.Infrastructure;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Cli;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Add;

[TestFixture]
internal sealed class AddCommandTests
{
    private readonly Mock<IProjectFileNamesLocator> _projectFileNameLocator = new();
    private CommandAppTester _app = null!;
    private TestConsole _console = null!;
    private ChangesetsRepository _changesetsRepository = null!;

    [SetUp]
    public void SetUp()
    {
        CleanUp();

        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
        serviceCollection.AddSingleton(_projectFileNameLocator.Object);
        _changesetsRepository = new ChangesetsRepository();
        serviceCollection.AddSingleton<IChangesetsRepository>(_changesetsRepository);

        TypeRegistrar typeRegistrar = new(serviceCollection);
        _app = new(typeRegistrar);
        _app.Configure(config =>
        {
            config
                .SetApplicationName("changeset")
                .SetExceptionHandler(ExceptionHandler.Handle);
        });
    }

    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

    [Test]
    public async Task AddCommand_HappyPath_ChangesetIsCreated()
    {
        // Arrange
        _projectFileNameLocator.Setup(x => x.GetProjectFileNames(It.IsAny<string>())).Returns(["A", "B"]);

        _console = new();
        _console.Interactive();

        // MultiSelectionPrompt - Select 2nd project 'B'
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.Spacebar);
        _console.Input.PushKey(ConsoleKey.Enter);

        // SelectionPrompt - Select BumpType 'Minor'
        _console.Input.PushKey(ConsoleKey.DownArrow);
        _console.Input.PushKey(ConsoleKey.Enter);

        // TextPrompt - Describe changes
        string expectedDescription = "Test example description";
        _console.Input.PushTextWithEnter(expectedDescription);

        // Act
        _app.SetDefaultCommand<InitChangesetCommand>();
        await _app.RunAsync();
        _app.SetDefaultCommand<AddChangesetCommand>();
        CommandAppResult result = _app.RunWithCustomConsole([], _console);

        // Assert
        result.ExitCode.Should().Be(ResultCodes.Success, result.Output);
        result.Output.Should().MatchRegex(@"Changeset file '[a-z]+\.md' for your branch has been created\.\s*$");

        ChangesetFile[] changesetFiles = await _changesetsRepository.GetChangesetsAsync(Constants.ChangesetDirectoryFullPath);
        changesetFiles.Should().HaveCount(1);

        ChangesetFile changesetFile = changesetFiles[0];
        changesetFile.ChangedModuleNames.Should().HaveCount(1);
        changesetFile.ChangedModuleNames.Should().Contain("B");
        changesetFile.BumpType.Should().Be(BumpType.Minor);
        changesetFile.Description.Should().Be(expectedDescription);
    }

    [Test]
    public void AddCommand_NotInitialized_ReturnsErrorCodeWithMessage()
    {
        // Arrange
        _app.SetDefaultCommand<AddChangesetCommand>();

        // Act
        CommandAppResult result = _app.Run();

        // Assert
        result.ExitCode.Should().Be(ResultCodes.NotInitialized, because: result.Output);
    }

    private static void CleanUp()
    {
        if (Directory.Exists(Constants.ChangesetDirectoryName))
        {
            Directory.Delete(Constants.ChangesetDirectoryName, true);
        }
    }
}

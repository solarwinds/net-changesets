using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWinds.Changesets.Commands.Init;
using SolarWinds.Changesets.Commands.Status;
using SolarWinds.Changesets.Infrastructure;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Status;

[TestFixture]
internal sealed class StatusChangesetCommandTests
{
    private CommandAppTester _app;

    private readonly Mock<IChangesetsRepository> _changesetsRepositoryMock = new();

    [SetUp]
    public void SetUp()
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
        serviceCollection.AddSingleton(_changesetsRepositoryMock.Object);

        TypeRegistrar typeRegistrar = new(serviceCollection);
        _app = new(typeRegistrar);
        _app.SetDefaultCommand<InitChangesetCommand>();
        _app.Run();
        _app.SetDefaultCommand<StatusChangesetCommand>();
    }

    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

    [Test]
    public void StatusChangesetCommand_WhenChangesetExists_FinishesWithNoError()
    {
        _changesetsRepositoryMock.Setup(x => x.GetChangesetsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ChangesetFile[]>([new ChangesetFile([""], BumpType.None, "")]));

        CommandAppResult result = _app.Run();

        Assert.That(result.ExitCode, Is.EqualTo(ResultCodes.Success));
    }

    [Test]
    public void StatusChangesetCommand_WhenNoChangesetExists_FinishesWithError()
    {
        _changesetsRepositoryMock.Setup(x => x.GetChangesetsAsync(It.IsAny<string>()))
            .Returns(Task.FromResult<ChangesetFile[]>([]));

        CommandAppResult result = _app.Run();

        Assert.That(result.ExitCode, Is.EqualTo(ResultCodes.NoChangesetsFound));
    }

    private static void CleanUp()
    {
        if (Directory.Exists(Constants.ChangesetDirectoryName))
        {
            Directory.Delete(Constants.ChangesetDirectoryName, true);
        }
    }
}

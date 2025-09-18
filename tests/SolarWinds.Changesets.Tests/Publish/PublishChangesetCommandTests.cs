using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolarWinds.Changesets.Commands.Init;
using SolarWinds.Changesets.Commands.Publish;
using SolarWinds.Changesets.Infrastructure;
using SolarWinds.Changesets.Services;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Publish;

[TestFixture]
internal sealed class PublishChangesetCommandTests
{
    private readonly Mock<IConfigurationService> _configurationServiceMock = new();
    private readonly Mock<IGitService> _gitServiceMock = new();
    private readonly Mock<IDotnetService> _dotnetServiceMock = new();

    private CommandAppTester _app;

    [SetUp]
    public void SetUp()
    {
        CleanUp();

        ServiceCollection serviceCollection = new();
        serviceCollection.AddSingleton(_configurationServiceMock.Object);
        serviceCollection.AddSingleton(_gitServiceMock.Object);
        serviceCollection.AddSingleton(_dotnetServiceMock.Object);

        _configurationServiceMock.Setup(x => x.GetConfigAsync(It.IsAny<string>())).Returns(Task.FromResult(new ChangesetConfig()));

        TypeRegistrar typeRegistrar = new(serviceCollection);
        _app = new(typeRegistrar);
        _app.SetDefaultCommand<InitChangesetCommand>();
        _app.Run();
        _app.SetDefaultCommand<PublishChangesetCommand>();
    }

    [TearDown]
    public void TearDown()
    {
        CleanUp();
    }

    [Test]
    public void PublishChangesetCommand_CompletesSuccesfully_HappyPath()
    {
        _gitServiceMock.Setup(x => x.GetDiff(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput(["A.csproj", "B.csproj"], 0)));
        _dotnetServiceMock.Setup(x => x.Pack(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput(["Project A packed", "Project B packed"], 0)));
        _dotnetServiceMock.Setup(x => x.Publish(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput(["Project A published", "Project B published"], 0)));

        CommandAppResult result = _app.Run();

        Assert.That(result.ExitCode, Is.EqualTo(ResultCodes.Success));
    }

    [Test]
    public void PublishChangesetCommand_WhenNoCsprojToPublish_FinishesWithError()
    {
        _gitServiceMock.Setup(x => x.GetDiff(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput([], 0)));

        CommandAppResult result = _app.Run();

        Assert.That(result.ExitCode, Is.EqualTo(ResultCodes.NoProjectToPublish));
    }

    [Test]
    public void PublishChangesetCommand_WhenPublishFails_FinishesWithError()
    {
        _gitServiceMock.Setup(x => x.GetDiff(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput(["A.csproj", "B.csproj"], 0)));
        _dotnetServiceMock.Setup(x => x.Pack(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput(["Project A packed", "Project B packed"], 0)));
        _dotnetServiceMock.Setup(x => x.Publish(It.IsAny<string>())).Returns(Task.FromResult(new ProcessOutput(["Some error ..."], 1)));

        CommandAppResult result = _app.Run();

        Assert.That(result.ExitCode, Is.EqualTo(ResultCodes.UnexpectedException));
    }

    private static void CleanUp()
    {
        if (Directory.Exists(Constants.ChangesetDirectoryName))
        {
            Directory.Delete(Constants.ChangesetDirectoryName, true);
        }
    }
}

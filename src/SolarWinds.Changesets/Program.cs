using Microsoft.Extensions.DependencyInjection;
using SolarWinds.Changesets.Commands.Add;
using SolarWinds.Changesets.Commands.Init;
using SolarWinds.Changesets.Commands.Publish;
using SolarWinds.Changesets.Commands.Status;
using SolarWinds.Changesets.Commands.Version;
using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Infrastructure;
using SolarWinds.Changesets.Services;
using SolarWinds.Changesets.Shared;
using Spectre.Console.Cli;

ServiceCollection serviceCollection = new();

// We do not need to register AnsiConsole.Console under IAnsiConsole. It is registered for us by CommandApp!
serviceCollection.AddSingleton<IConfigurationService, ConfigurationService>();
serviceCollection.AddSingleton<IProjectFileNamesLocator, CsharpProjectFileNamesLocator>();
serviceCollection.AddSingleton<IChangesetsRepository, ChangesetsRepository>();
serviceCollection.AddSingleton<IChangelogFileWriter, ChangelogFileWriter>();
serviceCollection.AddSingleton<IChangelogGenerator, ChangelogGenerator>();
serviceCollection.AddSingleton<ICsProjectsRepository, CsProjectsRepository>();
serviceCollection.AddSingleton<IProcessExecutor, ProcessExecutor>();
serviceCollection.AddSingleton<IGitService, GitService>();
serviceCollection.AddSingleton<IDotnetService, DotnetService>();

TypeRegistrar typeRegistrar = new(serviceCollection);

CommandApp<AddChangesetCommand> app = new(typeRegistrar);

app.Configure(config =>
{
    config
        .SetApplicationName("changeset")
        .UseAssemblyInformationalVersionWithoutSourceRevisionId()
        .SetExceptionHandler(ExceptionHandler.Handle);

    config
        .AddCommand<InitChangesetCommand>(InitChangesetCommand.Name)
        .WithDescription(InitChangesetCommand.Description)
        .WithExample("changeset", "init")
        ;

    config
        .AddCommand<AddChangesetCommand>(AddChangesetCommand.Name)
        .WithDescription(AddChangesetCommand.Description)
        .WithExample("changeset", "add")
        ;

    config
        .AddCommand<VersionChangesetCommand>(VersionChangesetCommand.Name)
        .WithDescription(VersionChangesetCommand.Description)
        .WithExample("changeset", "version")
        ;

    config
        .AddCommand<PublishChangesetCommand>(PublishChangesetCommand.Name)
        .WithDescription(PublishChangesetCommand.Description)
        .WithExample("changeset", "publish")
        ;

    config
        .AddCommand<StatusChangesetCommand>(StatusChangesetCommand.Name)
        .WithDescription(StatusChangesetCommand.Description)
        .WithExample("changeset", "status")
        ;
});

return app.Run(args);

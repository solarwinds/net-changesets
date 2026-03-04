using SolarWinds.Changesets.Commands.Publish.Services;
using SolarWinds.Changesets.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Commands.Publish;

/// <summary>
/// Creates new nuget package and publishes into predefined nuget source.
/// </summary>
/// <remarks>
/// Because this command assumes that the last commit is the release commit, you should not commit
/// any changes between calling version and publish. These commands are separate to enable you to
/// check if the release changes are accurate.
/// </remarks>
internal sealed class PublishChangesetCommand : ConfigurationCommandBase
{
    private readonly IAnsiConsole _console;
    private readonly IGitService _gitService;
    private readonly IDotnetService _dotnetService;

    public static string Name { get; } = "publish";

    public static string Description { get; } = "This publishes changes to specified nuget repository.";

    public PublishChangesetCommand(
        IAnsiConsole console,
        IConfigurationService configurationService,
        IGitService gitService,
        IDotnetService dotnetService
        ) : base(configurationService)
    {
        _console = console;
        _gitService = gitService;
        _dotnetService = dotnetService;
    }

    public override async Task<int> ExecuteCommandAsync(CommandContext context)
    {
        _console.WriteLine("Determining projects to publish ...");
        _console.WriteLine();

        ProcessOutput processOutput = await _gitService.GetDiff(Constants.WorkingDirectoryFullPath, ChangesetConfig.SourcePath);
        List<string> changedProjectNames = processOutput.Output
            .Where(x => x.EndsWith(".csproj", StringComparison.Ordinal) || x.EndsWith(".fsproj", StringComparison.Ordinal))
            .ToList();

        if (changedProjectNames.Count == 0)
        {
            _console.MarkupLine("[red]There are no projects to be published.[/]");
            return ResultCodes.NoProjectToPublish;
        }

        _console.MarkupLines(changedProjectNames);

        _console.WriteLine();
        _console.WriteLine("Packing ...");
        _console.WriteLine();

        foreach (string projectName in changedProjectNames)
        {
            string projectFullPath = Path.Join(Constants.WorkingDirectoryFullPath, projectName);

            ProcessOutput packOutput = await _dotnetService.Pack(projectFullPath);

            if (packOutput.ExitCode == 0)
            {
                _console.WriteLine(packOutput.Output.Last());
            }
            else
            {
                _console.MarkupLine("[red]An error occurred during packing nuget packages.[/]");
                _console.MarkupLines(packOutput.Output, "red italic");
                return ResultCodes.UnexpectedException;
            }
        }

        _console.WriteLine();
        _console.WriteLine("Publishing ...");
        _console.WriteLine();

        ProcessOutput publishOutput = await _dotnetService.Publish(ChangesetConfig.PackageSource);

        if (publishOutput.ExitCode == 0)
        {
            _console.MarkupLines(publishOutput.Output);
            _console.WriteLine();
            _console.MarkupLine("[green]Changeset publish finished.[/]");
            return ResultCodes.Success;
        }
        else
        {
            _console.MarkupLine("[red]An error occurred during publishing nuget packages.[/]");
            _console.MarkupLines(publishOutput.Output, "red italic");
            return ResultCodes.UnexpectedException;
        }
    }
}

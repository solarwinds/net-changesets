using SolarWinds.Changesets.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Commands.Status;

/// <summary>
/// Checks if the .changesets directory contains any changesets.
/// </summary>
internal sealed class StatusChangesetCommand : ConfigurationCommandBase
{
    private readonly IAnsiConsole _console;
    private readonly IChangesetsRepository _changesetsRepository;

    /// <summary>
    /// Name of the command.
    /// </summary>
    public static string Name { get; } = "status";

    /// <summary>
    /// Description of the command.
    /// </summary>
    public static string Description { get; } = "Provides information about the changesets that currently exist. If there are no changesets present, it exits with an error status code.";

    /// <summary>
    /// Initializes a new instance of the <see cref="StatusChangesetCommand"/> class.
    /// </summary>
    /// <param name="configurationService">The changeset configuration service.</param>
    /// <param name="console">The console.</param>
    /// <param name="changesetsRepository">The changesets repository.</param>
    public StatusChangesetCommand(
        IConfigurationService configurationService,
        IAnsiConsole console,
        IChangesetsRepository changesetsRepository) : base(configurationService)
    {
        _console = console;
        _changesetsRepository = changesetsRepository;
    }

    /// <summary>
    /// Executes Status command.
    /// </summary>
    /// <param name="context">Command context.</param>
    /// <returns>
    /// An integer indicating the result of the initialization:
    /// -  0: Positive number of changesets were found.
    /// -  -2: No changesets were found
    /// </returns>
    public override async Task<int> ExecuteCommandAsync(CommandContext context)
    {
        ChangesetFile[] changesets = await _changesetsRepository.GetChangesetsAsync(Constants.ChangesetDirectoryFullPath);

        if (changesets.Length == 0)
        {
            _console.WriteLine($"There were no changesets found.");

            return ResultCodes.NoChangesetsFound;
        }

        _console.WriteLine($"There {(changesets.Length == 1 ? "was" : "were")} {changesets.Length} changeset{(changesets.Length == 0 ? "" : "s")} found");

        return ResultCodes.Success;
    }
}

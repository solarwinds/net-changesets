using SolarWinds.Changesets.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Commands.Init;

/// <summary>
/// This command sets up the `.changeset` folder and it generates a default config file.
/// The config file includes the default options.
/// You should run this command once when you are setting up changesets.
/// </summary>
/// <remarks>
/// If you run this command a second time, it will check whether the .changeset folder already exists. It will not overwrite any existing files.
/// The command verifies the presence of both the .changeset folder and config.json. If they do not exist, it generates them.
/// </remarks>
internal sealed class InitChangesetCommand : AsyncCommand
{
    private readonly IAnsiConsole _console;
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// Name of the command.
    /// </summary>
    public static string Name { get; } = "init";

    /// <summary>
    /// Description of the command.
    /// </summary>
    public static string Description { get; } = "Sets up the .changeset folder and generates a default config file. You should run this command once when you are setting up changesets.";

    /// <summary>
    /// Initializes a new instance of the <see cref="InitChangesetCommand"/> class.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="configurationService">The changeset configuration service.</param>
    public InitChangesetCommand(
        IAnsiConsole console,
        IConfigurationService configurationService
        )
    {
        _console = console;
        _configurationService = configurationService;
    }

    /// <summary>
    /// Executes Init command.
    /// </summary>
    /// <param name="context">Command context.</param>
    /// <returns>
    /// An integer indicating the result of the initialization:
    /// -  0: Initialization succeeded.
    /// -  1: Directory existed but config file was missing; default config created.
    /// -  2: Changesets already initialized; no further action needed.
    /// </returns>
    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        if (Directory.Exists(Constants.ChangesetDirectoryFullPath))
        {
            if (File.Exists(Constants.ChangesetConfigFileFullPath))
            {
                _console.MarkupLine("[yellow]warn[/] It looks like you already have changesets initialized. You should be able to run changeset commands no problems.");
                return ResultCodes.AlreadyInitialized;
            }

            _console.MarkupLine("[red]error[/] It looks like you don't have a config file.");

            _console.MarkupLine("[blue]info[/] The default config file will be written at `.changeset/config.json`");

            await _configurationService.CreateDefaultAsync(Constants.ChangesetConfigFileFullPath);
            return ResultCodes.ConfigFileWasGenerated;
        }

        Directory.CreateDirectory(Constants.ChangesetDirectoryFullPath);
        await _configurationService.CreateDefaultAsync(Constants.ChangesetConfigFileFullPath);
        ReportChangesetsInitialized();

        return ResultCodes.Success;
    }

    private void ReportChangesetsInitialized()
    {
        _console.MarkupLine("Thanks for choosing [bold green]net-changesets[/] to help manage your versioning and publishing");

        _console.WriteLine();

        _console.MarkupLine("You should be set up to start using net-changesets now!");

        _console.WriteLine();

        _console.MarkupLine("A '.changeset' folder containing a default [blue]config.json[/] file has been created for you.");
    }
}

using System.Security.Cryptography;
using SolarWinds.Changesets.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Commands.Add;

/// <summary>
/// This is the main command people use to interact with the changesets.
/// This command will ask you a series of questions, first about what packages you want to release,
/// then what semver bump type for chosen packages, then it will ask for a summary of the entire changeset.
/// The final step will show the changeset it will generate and confirm that you want to add it.
/// Once confirmed, the changeset will be written as a Markdown file that contains the summary and
/// YAML front matter which stores the packages that will be released and the semver bump types for them.
/// </summary>
/// <remarks>
/// This is the default command when running Changesets without specifying a command.
/// The changeset file name is randomly generated using lowercase letters from the English alphabet.
/// </remarks>
internal sealed class AddChangesetCommand : ConfigurationCommandBase
{
    private const string MarkDownExtension = ".md";

    private readonly IAnsiConsole _console;
    private readonly IProjectFileNamesLocator _projectFileNamesLocator;
    private readonly IChangesetsRepository _changesetsRepository;

    /// <summary>
    /// Name of the command.
    /// </summary>
    public static string Name { get; } = "add";

    /// <summary>
    /// Description of the command.
    /// </summary>
    public static string Description { get; } = "Used by contributors to add information about their changes by creating changeset files.";

    /// <summary>
    /// Initializes a new instance of the <see cref="AddChangesetCommand"/> class.
    /// </summary>
    /// <param name="console">The console.</param>
    /// <param name="configurationService">The changesets configuration service.</param>
    /// <param name="projectFileNamesLocator">The project file names locator.</param>
    /// <param name="changesetsRepository">The changesets repository.</param>
    public AddChangesetCommand(
        IAnsiConsole console,
        IConfigurationService configurationService,
        IProjectFileNamesLocator projectFileNamesLocator,
        IChangesetsRepository changesetsRepository
        ) : base(configurationService)
    {
        _console = console;
        _projectFileNamesLocator = projectFileNamesLocator;
        _changesetsRepository = changesetsRepository;
    }

    /// <summary>
    /// Executes Add command.
    /// </summary>
    /// <param name="context">Command context.</param>
    /// <returns>
    /// An integer indicating the result of the initialization:
    /// -   0: Changeset file created successfully.
    /// -  -1: Changesets is not initialized.
    /// </returns>
    public override async Task<int> ExecuteCommandAsync(CommandContext context)
    {
        string[] projectNames = _projectFileNamesLocator.GetProjectFileNames(Path.Combine(Constants.WorkingDirectoryFullPath, ChangesetConfig.SourcePath));

        string[] selectedProjects = _console.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Please select [green]project[/]?")
                .PageSize(20)
                .MoreChoicesText("[grey](Move up and down to reveal more projects)[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to select a project, [green]<enter>[/] to accept)[/]")
                .AddChoices(projectNames)
            ).ToArray();

        string bumpType = _console.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select [green]bump type[/]?")
                .PageSize(10)
                // Skipping enum 'None = 0', that is not relevant to user
                .AddChoices(Enum.GetValues<BumpType>().Select(bt => bt.ToString()).Skip(1))
            );

        string description = _console.Prompt(
            new TextPrompt<string>("Please describe the changes done in the scope of your PR:"));

        string changesetFileFullPath = GetChangesetFileFullPath(Constants.ChangesetDirectoryFullPath);

        await _changesetsRepository.CreateChangesetAsync(selectedProjects, changesetFileFullPath, bumpType, description);

        _console.MarkupLine($"Changeset file [yellow]'{Path.GetFileName(changesetFileFullPath)}'[/] for your branch has been created.");

        return ResultCodes.Success;
    }

    private static string GetChangesetFileFullPath(string changesetFolderPath)
    {
        string changesetFileFullPath;
        do
        {
            string changesetFileName = GenerateRandomWord(15);
            string changesetFileNameWithExtension = Path.ChangeExtension(changesetFileName, MarkDownExtension);
            changesetFileFullPath = Path.Combine(changesetFolderPath, changesetFileNameWithExtension);
        } while (File.Exists(changesetFileFullPath));

        return changesetFileFullPath;
    }

    private static string GenerateRandomWord(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        return new string(
            Enumerable.Range(0, length)
                .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)])
                .ToArray()
        );
    }
}

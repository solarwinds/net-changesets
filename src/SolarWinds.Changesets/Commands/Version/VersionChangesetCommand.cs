using SolarWinds.Changesets.Commands.Version.Helpers;
using SolarWinds.Changesets.Shared;
using Spectre.Console;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Commands.Version;

/// <summary>
/// This is one of two commands responsible for releasing packages. The version command takes changesets that
/// have been made and updates versions and dependencies of packages, as well as writing changelogs. It is responsible
/// for all file changes to versions before publishing
/// </summary>
internal sealed class VersionChangesetCommand : ConfigurationCommandBase
{
    private readonly IAnsiConsole _console;
    private readonly IChangesetsRepository _changesetsRepository;
    private readonly ICsProjectsRepository _csProjFileHelper;
    private readonly IChangelogGenerator _changelogGenerator;
    private readonly IChangelogFileWriter _changelogFileWriter;

    public static string Name { get; } = "version";
    public static string Description { get; }
        = "Takes existing changeset files and updates versions and dependencies of packages, as well as writing changelogs.";

    public VersionChangesetCommand(
        IAnsiConsole console,
        IConfigurationService configurationService,
        IChangesetsRepository changesetsRepository,
        ICsProjectsRepository csProjFileHelper,
        IChangelogGenerator changelogGenerator,
        IChangelogFileWriter changelogFileWriter
        ) : base(configurationService)
    {
        _console = console;
        _changesetsRepository = changesetsRepository;
        _csProjFileHelper = csProjFileHelper;
        _changelogGenerator = changelogGenerator;
        _changelogFileWriter = changelogFileWriter;
    }

    public override async Task<int> ExecuteCommandAsync(CommandContext context)
    {
        ChangesetFile[] changesets = await _changesetsRepository.GetChangesetsAsync(Constants.ChangesetDirectoryFullPath);

        if (changesets.Length == 0)
        {
            _console.WriteLine("No unreleased changesets found, exiting.");
            return -1;
        }

        CsProject[] csProjects = _csProjFileHelper.GetCsProjects(ChangesetConfig);
        IEnumerable<ModuleChangelog> changelogs = _changelogGenerator.GetProcessedChangelogs(changesets, csProjects);

        await _changelogFileWriter.GenerateChangelogFilesAsync(changelogs);
        await _csProjFileHelper.UpdateCsProjectsVersionAsync(changelogs);
        _changesetsRepository.DeleteChangesets(Constants.ChangesetDirectoryFullPath);

        _console.WriteLine("All files have been updated. Review them and commit at your leisure.");

        string message = $"Affected projects: '{string.Join(", ", changelogs.Select(x => Path.GetFileNameWithoutExtension(x.ModuleCsProjFilePath)))}'";

        _console.WriteLine(message);

        return 0;
    }
}

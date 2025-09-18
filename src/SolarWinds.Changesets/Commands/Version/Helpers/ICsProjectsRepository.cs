using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

internal interface ICsProjectsRepository
{
    /// <summary>
    /// Goes through all the .csproj files in subfolders defined with <c>SourcePath</c> in the <see cref="ChangesetConfig"/>,
    /// parses their <c>Version</c> and <c>ProjectReference</c> attributes and returns a collection of <see cref="CsProject"/>.
    /// </summary>
    /// <param name="changesetConfig">Path to changesets configuration file</param>
    /// <returns>Collection of <see cref="CsProject"/></returns>
    CsProject[] GetCsProjects(ChangesetConfig changesetConfig);

    /// <summary>
    /// Replaces <c>Version</c> attribute of appropriate .csproj files.
    /// </summary>
    /// <param name="changelogs">Changelog defined with path to project file and new version</param>
    /// <returns><see cref="Task"/> representing the operation</returns>
    Task UpdateCsProjectsVersionAsync(IEnumerable<ModuleChangelog> changelogs);
}

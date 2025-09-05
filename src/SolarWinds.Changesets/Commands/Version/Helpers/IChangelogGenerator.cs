using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

internal interface IChangelogGenerator
{
    /// <summary>
    /// Generates changelogs for projects and their dependent projects.
    /// </summary>
    /// <param name="changesets"></param>
    /// <param name="csProjects"></param>
    /// <returns>Collection of <see cref="ModuleChangelog"/></returns>
    IEnumerable<ModuleChangelog> GetProcessedChangelogs(ChangesetFile[] changesets, IEnumerable<CsProject> csProjects);
}

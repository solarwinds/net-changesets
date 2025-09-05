namespace SolarWinds.Changesets.Commands.Version.Helpers;

internal interface IChangelogFileWriter
{
    /// <summary>
    /// Produces new or amends existing changelog file for every affected project file
    /// </summary>
    /// <param name="moduleChangelogs">Collection of <see cref="ModuleChangelog"/></param>
    /// <returns><see cref="Task"/> representing the operation</returns>
    Task GenerateChangelogFilesAsync(IEnumerable<ModuleChangelog> moduleChangelogs);
}

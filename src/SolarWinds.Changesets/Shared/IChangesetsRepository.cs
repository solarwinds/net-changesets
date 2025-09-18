namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Defines methods for accessing and managing changesets.
/// </summary>
public interface IChangesetsRepository
{
    /// <summary>
    /// Creates changeset file in the changeset working directory.
    /// </summary>
    /// <param name="updatedModules">Updated modules.</param>
    /// <param name="path">Location of new changeset file.</param>
    /// <param name="bumpType">Bump type of changes.</param>
    /// <param name="description">Description of changes.</param>
    /// <returns></returns>
    Task CreateChangesetAsync(string[] updatedModules, string path, string bumpType, string description);

    /// <summary>
    /// Gets all changeset files in the changeset working directory.
    /// </summary>
    /// <param name="changesetDirectoryFullPath">Path to changeset working directory</param>
    /// <returns>Array of <see cref="ChangesetFile"/></returns>
    Task<ChangesetFile[]> GetChangesetsAsync(string changesetDirectoryFullPath);

    /// <summary>
    /// Deletes all changeset files in the changeset working directory.
    /// </summary>
    /// <param name="changesetDirectoryFullPath">Path to changeset working directory</param>
    void DeleteChangesets(string changesetDirectoryFullPath);
}

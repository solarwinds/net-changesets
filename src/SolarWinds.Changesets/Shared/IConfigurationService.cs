namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Provides functionality to manage changeset configuration files.
/// </summary>
/// <remarks>
/// This service allows creating a default configuration file and retrieving an existing configuration file.
/// It uses JSON serialization and deserialization to handle the configuration data.
/// </remarks>
public interface IConfigurationService
{

    /// <summary>
    /// Creates a default changeset configuration file at the specified path.
    /// </summary>
    /// <param name="path">Path where the configuration file should be created</param>
    /// <returns><see cref="Task"/> representing the async operation</returns>
    Task CreateDefaultAsync(string path);

    /// <summary>
    /// Gets changeset configuration file.
    /// </summary>
    /// <param name="path"> Path to changeset config file</param>
    /// <returns><see cref="Task"/> representing the async operation, containing <see cref="ChangesetConfig"/></returns>
    Task<ChangesetConfig> GetConfigAsync(string path);
}

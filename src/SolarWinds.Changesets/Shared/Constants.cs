namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Provides application-wide constant values of file names and file path.
/// </summary>
public static class Constants
{
    /// <summary>
    /// The file name of the changelog file.
    /// </summary>
    public const string ChangelogFileName = "CHANGELOG.md";

    /// <summary>
    /// The name of the directory that contains changesets.
    /// </summary>
    public const string ChangesetDirectoryName = ".changeset";

    /// <summary>
    /// The file name of the configuration file.
    /// </summary>
    public const string ConfigFileName = "config.json";

    /// <summary>
    /// Gets the full path to the current working directory.
    /// </summary>
    public static string WorkingDirectoryFullPath { get; } = Environment.CurrentDirectory;

    /// <summary>
    /// Gets the full path to the changeset directory.
    /// </summary>
    public static string ChangesetDirectoryFullPath { get; } = Path.Combine(WorkingDirectoryFullPath, ChangesetDirectoryName);

    /// <summary>
    /// Gets the full path to the changeset configuration file.
    /// </summary>
    public static string ChangesetConfigFileFullPath { get; } = Path.Combine(ChangesetDirectoryFullPath, ConfigFileName);

    /// <summary>
    /// Gets the full path to the directory where NuGet packages are output.
    /// </summary>
    public static string NupkgOutputFullPath { get; } = Path.Combine(WorkingDirectoryFullPath, "nupkg");
}

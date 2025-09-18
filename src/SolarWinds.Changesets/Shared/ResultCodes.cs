namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Defines command result codes with documentation.
/// </summary>
public static class ResultCodes
{
    #region Success Codes

    /// <summary>
    /// Success.
    /// </summary>
    public const int Success = 0;

    /// <summary>
    /// '.changeset' exists, but config file is missing. Default config file was created.
    /// </summary>
    public const int ConfigFileWasGenerated = 1;

    /// <summary>
    /// Changeset is already initialized, there is nothing to do.
    /// </summary>
    public const int AlreadyInitialized = 2;

    /// <summary>
    /// There are no '.csproj' files with updated version.
    /// </summary>
    public const int NoProjectToPublish = 3;

    #endregion

    #region Error Codes

    /// <summary>
    /// Changeset is not initialized. Config file on path '.\.changeset\config.json' is missing.
    /// </summary>
    public const int NotInitialized = -1;

    /// <summary>
    /// No changesets were found.
    /// </summary>
    public const int NoChangesetsFound = -2;

    /// <summary>
    /// Unexpected exception occurred.
    /// </summary>
    public const int UnexpectedException = -10;

    #endregion
}

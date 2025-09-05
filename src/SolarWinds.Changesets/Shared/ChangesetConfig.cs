namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Changeset config.
/// </summary>
public sealed class ChangesetConfig
{
    /// <summary>
    /// Specify the relative path from the changeset command's execution folder to the location of the projects.
    /// </summary>
    /// <remarks>
    /// The default value is `src`.
    /// </remarks>
    public string SourcePath { get; init; } = "src";

    /// <summary>
    /// Specify the NuGet package source where the packages will be published.
    /// </summary>
    /// <remarks>
    /// Default value is `nuget`. It is official [nuget.org](https://www.nuget.org/) repository.
    /// </remarks>
    public string PackageSource { get; init; } = "nuget";
}

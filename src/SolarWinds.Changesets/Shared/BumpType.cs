namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Specifies the type of version bump to apply.
/// </summary>
public enum BumpType
{
    /// <summary>
    /// No version bump.
    /// </summary>
    None = 0,

    /// <summary>
    /// Patch version bump (e.g., 1.0.0 to 1.0.1).
    /// </summary>
    Patch = 1,

    /// <summary>
    /// Minor version bump (e.g., 1.0.0 to 1.1.0).
    /// </summary>
    Minor = 2,

    /// <summary>
    /// Major version bump (e.g., 1.0.0 to 2.0.0).
    /// </summary>
    Major = 3
}

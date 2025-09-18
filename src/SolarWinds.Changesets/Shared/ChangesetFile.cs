namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Represents changeset model.
/// </summary>
/// <param name="ChangedModuleNames">Changed module names.</param>
/// <param name="BumpType">Bump type.</param>
/// <param name="Description">Description.</param>
public sealed record ChangesetFile(ICollection<string> ChangedModuleNames, BumpType BumpType, string Description);

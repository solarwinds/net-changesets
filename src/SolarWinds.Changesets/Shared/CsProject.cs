namespace SolarWinds.Changesets.Shared;

internal sealed record CsProject(string Name, Semver Version, string[] ReferencedProjectNames, string CsprojFilePath);

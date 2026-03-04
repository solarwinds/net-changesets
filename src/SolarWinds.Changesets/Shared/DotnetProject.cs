namespace SolarWinds.Changesets.Shared;

internal sealed record DotnetProject(string Name, Semver Version, string[] ReferencedProjectNames, string ProjectFilePath);

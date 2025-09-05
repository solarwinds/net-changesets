using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Version;

internal sealed class ModuleChangelog
{
    public required string ModuleName { get; init; } = default!;
    public required string ModuleCsProjFilePath { get; init; } = default!;
    public required Semver CurrentVersion { get; init; } = default!;
    public ICollection<(string Description, BumpType BumpType)> Changes { get; init; } = [];
    public string ModuleDirectoryPath => Path.GetDirectoryName(ModuleCsProjFilePath)!;

    public Semver GetNewVersion()
    {
        Semver raisedSemver = new(CurrentVersion);
        BumpType highestBumpType = Changes.Max(x => x.BumpType);

        switch (highestBumpType)
        {
            case BumpType.Major:
                raisedSemver.RaiseMajor();
                break;
            case BumpType.Minor:
                raisedSemver.RaiseMinor();
                break;
            case BumpType.Patch:
                raisedSemver.RaisePatch();
                break;
        }

        return raisedSemver;
    }
}

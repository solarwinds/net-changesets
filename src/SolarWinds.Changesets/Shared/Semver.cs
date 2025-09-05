using SystemVersion = System.Version;

namespace SolarWinds.Changesets.Shared;

internal sealed class Semver
{
    public int Major { get; private set; }
    public int Minor { get; private set; }
    public int Patch { get; private set; }

    public Semver(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public Semver(Semver semver)
    {
        Major = semver.Major;
        Minor = semver.Minor;
        Patch = semver.Patch;
    }

    public void RaiseMajor()
    {
        Major++;
        Minor = 0;
        Patch = 0;
    }

    public void RaiseMinor()
    {
        Minor++;
        Patch = 0;
    }

    public void RaisePatch()
    {
        Patch++;
    }

    public override string ToString()
    {
        return $"{Major}.{Minor}.{Patch}";
    }

    public static Semver? FromString(string version)
    {
        if (!SystemVersion.TryParse(version, out SystemVersion? result))
        {
            return null;
        }

        return new(result.Major, result.Minor, result.Build);
    }
}

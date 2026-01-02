using System.Diagnostics.CodeAnalysis;
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

    /// <summary>
    /// Attempts to parse a version string into a <see cref="Semver"/> instance.
    /// </summary>
    /// <param name="version">The version string to parse (e.g., "1.2.3").</param>
    /// <param name="semver">When this method returns, contains the parsed <see cref="Semver"/> if parsing succeeded, or null if parsing failed.</param>
    /// <returns>True if the version string was successfully parsed; otherwise, false.</returns>
    public static bool TryParse(string version, [NotNullWhen(true)] out Semver? semver)
    {
        if (SystemVersion.TryParse(version, out SystemVersion? result))
        {
            semver = new(result.Major, result.Minor, result.Build);
            return true;
        }

        semver = null;
        return false;
    }

    /// <summary>
    /// Gets an empty <see cref="Semver"/> instance with version 0.0.0.
    /// </summary>
    /// <value>A <see cref="Semver"/> instance representing version 0.0.0.</value>
    public static Semver Empty { get; } = new(0, 0, 0);
}

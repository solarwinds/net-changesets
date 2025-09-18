using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Services;

/// <summary>
/// Provides Git-related services, such as retrieving file differences.
/// </summary>
public interface IGitService
{
    /// <summary>
    /// Retrieves a list of file names that have changed in the specified source path.
    /// </summary>
    /// <param name="sourcePath">The path to check for file differences.</param>
    /// <returns>Process output and exit code.</returns>
    Task<ProcessOutput> GetDiff(string sourcePath);
}

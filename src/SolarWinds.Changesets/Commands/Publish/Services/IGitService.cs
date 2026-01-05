using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Publish.Services;

/// <summary>
/// Provides Git-related services, such as retrieving file differences.
/// </summary>
public interface IGitService
{
    /// <summary>
    /// Retrieves a list of file names that have changed in the specified source path.
    /// </summary>
    /// <param name="workingDirectory">Working directory.</param>
    /// <param name="sourcePath">The path to check for file differences.</param>
    /// <returns>Process output and exit code.</returns>
    Task<ProcessOutput> GetDiff(string workingDirectory, string sourcePath);
}

using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Services;

/// <summary>
/// Provides functionality to interact with the .NET CLI for operations such as packing and publishing NuGet packages.
/// </summary>
public interface IDotnetService
{
    /// <summary>
    /// Packs a .NET project into a NuGet package.
    /// </summary>
    /// <param name="projectPath">The path to the project file to pack.</param>
    /// <returns>Process output and exit code.</returns>
    Task<ProcessOutput> Pack(string projectPath);

    /// <summary>
    /// Publishes NuGet packages to the specified package source.
    /// </summary>
    /// <param name="packageSource">The NuGet package source name.</param>
    /// <returns>Process output and exit code.</returns>
    /// <exception cref="ArgumentException">Thrown when the sourcePath is null or empty.</exception>
    /// <remarks>
    /// This method will push all NuGet packages (*.nupkg) from the output directory to the specified source.
    /// </remarks>
    Task<ProcessOutput> Publish(string packageSource);
}

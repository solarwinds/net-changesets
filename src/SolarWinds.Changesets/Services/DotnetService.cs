using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Services;

/// <inheritdoc />
public sealed class DotnetService : IDotnetService
{
    private readonly IProcessExecutor _processExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotnetService"/> class with the specified process executor.
    /// </summary>
    /// <param name="processExecutor"> The <see cref="IProcessExecutor"/> used to execute external processes. </param>
    public DotnetService(IProcessExecutor processExecutor)
    {
        _processExecutor = processExecutor;
    }

    /// <inheritdoc />
    public async Task<ProcessOutput> Pack(string projectPath)
    {
        return await _processExecutor.Execute(
            "dotnet",
            $"pack {projectPath} --output {Constants.NupkgOutputFullPath}",
            Constants.WorkingDirectoryFullPath
        );
    }

    /// <inheritdoc />
    public async Task<ProcessOutput> Publish(string packageSource)
    {
        if (string.IsNullOrEmpty(packageSource))
        {
            throw new ArgumentException("Source path cannot be null or empty.", nameof(packageSource));
        }

        string dotnetCommand = $"nuget push {Path.Join(Constants.NupkgOutputFullPath, "*.nupkg")}";

        dotnetCommand += $" --source {packageSource}";

        return await _processExecutor.Execute("dotnet", dotnetCommand, Constants.WorkingDirectoryFullPath);
    }
}

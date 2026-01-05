using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Publish.Services;

/// <inheritdoc />
public sealed class GitService : IGitService
{
    private readonly IProcessExecutor _processExecutor;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitService"/> class using the specified process executor.
    /// </summary>
    /// <param name="processExecutor">
    /// The <see cref="IProcessExecutor"/> implementation used to execute Git commands.
    /// </param>
    public GitService(IProcessExecutor processExecutor)
    {
        _processExecutor = processExecutor;
    }

    /// <inheritdoc />
    public async Task<ProcessOutput> GetDiff(string workingDirectory, string sourcePath)
    {
        return await _processExecutor.Execute(
            "git",
            $"diff --name-only HEAD~1 HEAD {sourcePath}",
            workingDirectory
        );
    }
}

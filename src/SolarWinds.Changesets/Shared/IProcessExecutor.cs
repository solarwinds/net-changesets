namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Executes external processes and captures their output and exit code.
/// </summary>
public interface IProcessExecutor
{
    /// <summary>
    /// Executes a process with the specified executable, arguments, and working directory.
    /// Captures both standard output and error streams.
    /// </summary>
    /// <param name="executable">The path to the executable to run.</param>
    /// <param name="arguments">The arguments to pass to the executable.</param>
    /// <param name="workingDirectoryPath">The working directory for the process.</param>
    /// <returns>A <see cref="ProcessOutput"/> object containing the output and exit code of the process.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the process cannot be started with the specified parameters.
    /// </exception>
    Task<ProcessOutput> Execute(string executable, string arguments, string workingDirectoryPath);
}

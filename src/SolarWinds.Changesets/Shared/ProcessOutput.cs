namespace SolarWinds.Changesets.Shared;

/// <summary>
/// Represents the output of a process execution, including its output lines and exit code.
/// </summary>
/// <param name="Output">The list of output lines from the process.</param>
/// <param name="ExitCode">The exit code of the process.</param>
public record ProcessOutput(ICollection<string> Output, int ExitCode);

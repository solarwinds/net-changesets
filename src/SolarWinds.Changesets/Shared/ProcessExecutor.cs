using System.Diagnostics;

namespace SolarWinds.Changesets.Shared;

/// <inheritdoc />
public class ProcessExecutor : IProcessExecutor
{
    /// <inheritdoc />
    public async Task<ProcessOutput> Execute(string executable, string arguments, string workingDirectoryPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = arguments,
            WorkingDirectory = workingDirectoryPath,
            RedirectStandardOutput = true,
            RedirectStandardInput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        List<string> output = [];

        using Process process = new() { StartInfo = startInfo };

        process.OutputDataReceived += (_, args) => WriteToOutput(output, args.Data);

        process.ErrorDataReceived += (_, args) => WriteToOutput(output, args.Data);

        process.Start();

        process.BeginOutputReadLine();

        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        return new ProcessOutput(output, process.ExitCode);
    }

    /// <summary>
    /// Writes a line of output to the provided list if the line is not null.
    /// </summary>
    /// <param name="output">The list to which the output will be added.</param>
    /// <param name="toWrite">The line of output to write.</param>
    private static void WriteToOutput(List<string> output, string? toWrite)
    {
        if (toWrite != null)
        {
            output.Add(toWrite);
        }
    }
}

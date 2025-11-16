using Spectre.Console;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Shared;

internal static class ExceptionHandler
{
    public static int Handle(Exception ex, ITypeResolver? _)
    {
        int returnCode = ResultCodes.UnexpectedException;

        if (ex is InitializationException)
        {
            AnsiConsole.MarkupLine("[red]error[/] The [yellow]changeset[/] tool is not initialized in this repository.");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Please run [yellow]changeset init[/] first in the root of the repository to configure the tool.");
            return ResultCodes.NotInitialized;
        }

        AnsiConsole.WriteException(ex);
        return returnCode;
    }
}

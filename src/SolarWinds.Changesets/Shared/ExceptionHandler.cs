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
            returnCode = ResultCodes.NotInitialized;
            AnsiConsole.MarkupLine("[red]Error:[/] The changesets tool is not initialized in this repository.");
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Please run [yellow]changeset init[/] first to configure the tool.");
            return returnCode;
        }

        AnsiConsole.WriteException(ex);
        return returnCode;
    }
}

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
        }

        AnsiConsole.WriteException(ex);
        return returnCode;
    }
}

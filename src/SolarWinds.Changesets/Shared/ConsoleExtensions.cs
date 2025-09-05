using Spectre.Console;

namespace SolarWinds.Changesets.Shared;

internal static class ConsoleExtensions
{
    public static void ChangesetsIsNotInitialized(this IAnsiConsole console)
    {
        const string errorMessage =
            """
            There is no .changeset folder.
            If this is the first time `changesets` have been used in this project, run `net-changeset init` to get set up.
            If you expect changesets to exist, check the Git history for when the folder was removed to ensure the configuration is not lost.
            """;
        console.MarkupLine($"[red]error[/] {errorMessage}");
    }

    /// <summary>
    /// Prints a list of messages to the console, optionally applying a Spectre.Console markup style to each message.
    /// </summary>
    /// <param name="console">The IAnsiConsole instance.</param>
    /// <param name="messages">The messages to print.</param>
    /// <param name="markupStyle">Optional Spectre.Console markup style (e.g., "red italic"). If null or empty, prints messages as-is.</param>
    public static void MarkupLines(this IAnsiConsole console, IEnumerable<string> messages, string markupStyle = "")
    {
        foreach (string message in messages)
        {
            if (string.IsNullOrEmpty(markupStyle))
            {
                console.MarkupLine(message);
            }
            else
            {
                console.MarkupLine($"[{markupStyle}]{message}[/]");
            }
        }
    }
}

using Spectre.Console;

namespace SolarWinds.Changesets.Shared;

internal static class ConsoleExtensions
{
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

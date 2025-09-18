using System.Reflection;
using Spectre.Console.Testing;

namespace SolarWinds.Changesets.Tests.Add;

internal static class CommandAppTesterExtensions
{
    /// <summary>
    /// This method is used to run the command app tester with a custom console.
    /// </summary>
    /// <Remarks>
    /// This can be deleted once we upgrade Spectre.Console.Testing to version 0.51.X or higher.
    /// </Remarks>
    /// <param name="tester">CommandAppTester instance.</param>
    /// <param name="args">CLI arguments.</param>
    /// <param name="console">Custom test console.</param>
    /// <returns>Command result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the command failed to run with the custom console.</exception>
    public static CommandAppResult RunWithCustomConsole(
        this CommandAppTester tester,
        string[] args,
        TestConsole console
        )
    {
        Type testerType = tester.GetType();

        MethodInfo? runMethod = testerType.GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Instance);

        if (runMethod == null)
        {
            throw new InvalidOperationException("The method 'Run' was not found.");
        }

        object[] parameters = [args, console, null!];

        object? result = runMethod.Invoke(tester, parameters);

        if (result is CommandAppResult commandAppResult)
        {
            return commandAppResult;
        }

        throw new InvalidOperationException("The method 'Run' returned unexpected type.");
    }
}

using System.Reflection;
using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Infrastructure;

internal static class SpectreConsoleConfiguratorExtensions
{
    /// <summary>
    /// Uses the version retrieved from the <see cref="AssemblyInformationalVersionAttribute"/> as the application's version.
    /// </summary>
    /// <remarks>
    /// The InformationalVersion contains version in following format: $(VersionPrefix)-$(VersionSuffix)+$(SourceRevisionId)
    /// We are only interested in the $(VersionPrefix)-$(VersionSuffix) part, so we split by '+' and take the first part.
    /// </remarks>
    /// <param name="configurator">The configurator.</param>
    /// <returns>A configurator that can be used to configure the application further.</returns>
    public static IConfigurator UseAssemblyInformationalVersionWithoutSourceRevisionId(this IConfigurator configurator)
    {
        ArgumentNullException.ThrowIfNull(configurator);

        configurator.Settings.ApplicationVersion = Assembly
            .GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')[0] ?? "Failed to retrieve version.";

        return configurator;
    }
}

using System.Text.RegularExpressions;
using System.Xml.Linq;
using SolarWinds.Changesets.Shared;
using Spectre.Console;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

/// <summary>
/// Csharp projects file system repository.
/// </summary>
internal sealed partial class CsProjectsRepository : ICsProjectsRepository
{
    private readonly IAnsiConsole _console;

    public CsProjectsRepository(IAnsiConsole console)
    {
        _console = console;
    }

    /// <inheritdoc />
    public CsProject[] GetCsProjects(ChangesetConfig changesetConfig)
    {
        List<CsProject> csProjects = [];

        string[] csprojFilePaths = GetCsprojFilePaths(Path.Join(Constants.WorkingDirectoryFullPath, changesetConfig.SourcePath));

        foreach (string csprojFilePath in csprojFilePaths)
        {
            XDocument csprojXDocument = XDocument.Load(csprojFilePath);
            string projectName = Path.GetFileNameWithoutExtension(csprojFilePath);

            XElement? projectVersionXElement = csprojXDocument.Descendants().SingleOrDefault(d => d.Name.LocalName == "Version");
            Semver? projectVersion = projectVersionXElement is not null
                ? Semver.FromString(projectVersionXElement.Value)
                : new(0, 0, 0);

            if (projectVersion is null)
            {
                _console.MarkupLine($"[yellow]Version {projectVersionXElement?.Value} could not be parsed " +
                    $"for project {projectName}. This may have unexpected consequences on the 'version' command![/]");
                continue;
            }

            // Find all ProjectReference elements
            string[] projectReferences = csprojXDocument
                .Descendants()
                .Where(d => d.Name.LocalName == "ProjectReference" && d.Attribute("Include") is not null)
                .Select(i => Path.GetFileNameWithoutExtension(i.Attribute("Include")!.Value.Replace('\\', '/')))
                .ToArray();

            csProjects.Add(new(projectName, projectVersion, projectReferences, csprojFilePath));

        }

        return csProjects.ToArray();
    }

    /// <inheritdoc/>
    public async Task UpdateCsProjectsVersionAsync(IEnumerable<ModuleChangelog> changelogs)
    {
        foreach (ModuleChangelog changelog in changelogs)
        {
            await UpdateCsProjectVersionAsync(changelog.ModuleCsProjFilePath, changelog.GetNewVersion().ToString());
        }
    }

    private static string[] GetCsprojFilePaths(string rootFilePath)
    {
        return Directory.GetFiles(rootFilePath, "*.csproj", SearchOption.AllDirectories);
    }

    private async Task UpdateCsProjectVersionAsync(string moduleCsProjFilePath, string newVersion)
    {
        string csProjContent = await File.ReadAllTextAsync(moduleCsProjFilePath);

        if (!csProjContent.Contains("<Version>", StringComparison.Ordinal))
        {
            _console.MarkupLine($"[red]The csproj file does not contain Version element: {moduleCsProjFilePath}. Skipping[/]");

            return;
        }

        string newVersionContent = VersionRegex().Replace(csProjContent, match => $"{match.Groups[1].Value}{newVersion}{match.Groups[3].Value}");

        await File.WriteAllTextAsync(moduleCsProjFilePath, newVersionContent);
    }

    [GeneratedRegex(@"(<Version>)(.*?)(</Version>)")]
    private static partial Regex VersionRegex();
}

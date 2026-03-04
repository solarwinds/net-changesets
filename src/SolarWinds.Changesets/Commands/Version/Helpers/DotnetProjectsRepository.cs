using System.Text.RegularExpressions;
using System.Xml.Linq;
using SolarWinds.Changesets.Shared;
using Spectre.Console;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

/// <summary>
/// .NET projects (.csproj and .fsproj) file system repository.
/// </summary>
internal sealed partial class DotnetProjectsRepository : IDotnetProjectsRepository
{
    /// <summary>
    /// The file extension patterns used to identify .NET project files.
    /// </summary>
    private static readonly string[] s_projectFileExtensions = ["*.csproj", "*.fsproj"];

    private readonly IAnsiConsole _console;

    public DotnetProjectsRepository(IAnsiConsole console)
    {
        _console = console;
    }

    /// <inheritdoc />
    public DotnetProject[] GetProjects(ChangesetConfig changesetConfig)
    {
        List<DotnetProject> projects = [];

        string[] projectFilePaths = GetProjectFilePaths(Path.Join(Constants.WorkingDirectoryFullPath, changesetConfig.SourcePath));

        foreach (string projectFilePath in projectFilePaths)
        {
            XDocument projectXDocument = XDocument.Load(projectFilePath);
            string projectName = Path.GetFileNameWithoutExtension(projectFilePath);

            Semver projectVersion = GetProjectVersion(projectXDocument, projectName);
            if (projectVersion == Semver.Empty)
            {
                continue;
            }

            // Find all ProjectReference elements
            string[] projectReferences = projectXDocument
                .Descendants()
                .Where(d => d.Name.LocalName == "ProjectReference" && d.Attribute("Include") is not null)
                .Select(i => Path.GetFileNameWithoutExtension(i.Attribute("Include")!.Value.Replace('\\', '/')))
                .ToArray();

            projects.Add(new(projectName, projectVersion, projectReferences, projectFilePath));
        }

        return projects.ToArray();
    }

    /// <inheritdoc/>
    public async Task UpdateProjectsVersionAsync(IEnumerable<ModuleChangelog> changelogs)
    {
        foreach (ModuleChangelog changelog in changelogs)
        {
            await UpdateProjectVersionAsync(changelog.ModuleProjectFilePath, changelog.GetNewVersion().ToString());
        }
    }

    private static string[] GetProjectFilePaths(string rootFilePath)
    {
        return s_projectFileExtensions
            .SelectMany(extension => Directory.GetFiles(rootFilePath, extension, SearchOption.AllDirectories))
            .ToArray();
    }

    private async Task UpdateProjectVersionAsync(string moduleProjectFilePath, string newVersion)
    {
        string projectContent = await File.ReadAllTextAsync(moduleProjectFilePath);

        if (!projectContent.Contains("<Version>", StringComparison.Ordinal))
        {
            _console.MarkupLine($"[red]The project file does not contain Version element: {moduleProjectFilePath}. Skipping[/]");

            return;
        }

        string newVersionContent = VersionRegex().Replace(projectContent, match => $"{match.Groups[1].Value}{newVersion}{match.Groups[3].Value}");

        await File.WriteAllTextAsync(moduleProjectFilePath, newVersionContent);
    }

    private Semver GetProjectVersion(XDocument projectXDocument, string projectName)
    {
        XElement? projectVersionXElement = projectXDocument.Descendants().SingleOrDefault(d => d.Name.LocalName == "Version");

        if (projectVersionXElement is not null && Semver.TryParse(projectVersionXElement.Value, out Semver? parsedVersion))
        {
            return parsedVersion;
        }

        _console.MarkupLine($"[yellow]Version {projectVersionXElement?.Value} could not be parsed " +
            $"for project {projectName}. This may have unexpected consequences on the 'version' command![/]");

        return Semver.Empty;
    }

    [GeneratedRegex(@"(<Version>)(.*?)(</Version>)")]
    private static partial Regex VersionRegex();
}

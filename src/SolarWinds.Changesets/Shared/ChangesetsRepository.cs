using System.Text.RegularExpressions;

namespace SolarWinds.Changesets.Shared;

internal sealed partial class ChangesetsRepository : IChangesetsRepository
{
    /// <inheritdoc/>
    public async Task CreateChangesetAsync(
        string[] updatedModules, string path, string bumpType, string description)
    {
        string content = $"""
        ---
        {string.Join(Environment.NewLine, updatedModules.Select(module => $"\"{module}\": {bumpType}"))}
        ---
        
        {description}
        """;

        await File.WriteAllTextAsync(path, content);
    }

    /// <inheritdoc/>
    public async Task<ChangesetFile[]> GetChangesetsAsync(string changesetDirectoryFullPath)
    {
        ThrowIfDirectoryNotFound(changesetDirectoryFullPath);

        string[] changesetsFilesPaths = GetChangesetFilesPaths(changesetDirectoryFullPath);
        List<ChangesetFile> changesets = [];

        foreach (string changesetFilePath in changesetsFilesPaths)
        {
            string[] changesetFileLines = await File.ReadAllLinesAsync(changesetFilePath);
            changesets.Add(ParseChangesetFile(changesetFileLines));
        }

        return [.. changesets];
    }

    /// <inheritdoc/>
    public void DeleteChangesets(string changesetDirectoryFullPath)
    {
        ThrowIfDirectoryNotFound(changesetDirectoryFullPath);

        string[] changesetsFilesPaths = GetChangesetFilesPaths(changesetDirectoryFullPath);

        foreach (string changesetsFilePath in changesetsFilesPaths)
        {
            File.Delete(changesetsFilePath);
        }
    }

    private static ChangesetFile ParseChangesetFile(string[] fileLines)
    {
        int i = 1; // Skipping first line '---'
        ICollection<string> changedModules = [];
        string bumpTypeString;
        do
        {
            Match moduleMatch = ModuleRegex().Match(fileLines[i]);
            string changedModule = moduleMatch.Groups[1].Value;
            bumpTypeString = moduleMatch.Groups[2].Value;
            changedModules.Add(changedModule);
            i++;
        } while (fileLines[i] != "---");

        string description = string.Join(Environment.NewLine, fileLines.Skip(i + 2));

        if (!Enum.TryParse(bumpTypeString, true, out BumpType bumpType))
        {
            throw new FormatException($"Invalid bump type format: {bumpTypeString}.");
        }

        return new ChangesetFile(changedModules, bumpType, description);
    }

    private static string[] GetChangesetFilesPaths(string changesetDirectoryFullPath)
    {
        return Directory.GetFiles(changesetDirectoryFullPath, "*.md")
                        .Where(file => !file.EndsWith("README.md", StringComparison.OrdinalIgnoreCase))
                        .ToArray();
    }

    private static void ThrowIfDirectoryNotFound(string directoryFullPath)
    {
        if (!Directory.Exists(directoryFullPath))
        {
            throw new ArgumentException(
                $"Directory does not exist at the supplied path - '{directoryFullPath}'.", nameof(directoryFullPath));
        }
    }

    [GeneratedRegex(@"""([^""]+)""\s*:\s*(\w+)", RegexOptions.Compiled)]
    private static partial Regex ModuleRegex();
}

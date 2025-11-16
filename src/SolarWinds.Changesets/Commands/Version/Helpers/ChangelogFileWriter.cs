using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

internal sealed class ChangelogFileWriter : IChangelogFileWriter
{
    /// <inheritdoc/>
    public async Task GenerateChangelogFilesAsync(IEnumerable<ModuleChangelog> moduleChangelogs)
    {
        foreach (ModuleChangelog moduleChangelog in moduleChangelogs)
        {
            string changelogEntry = GenerateChangelogFile(moduleChangelog);

            string moduleChangelogFilePath = Path.Join(moduleChangelog.ModuleDirectoryPath, Constants.ChangelogFileName);

            if (Path.Exists(moduleChangelogFilePath))
            {
                List<string> changelogFileLines = (await File.ReadAllLinesAsync(moduleChangelogFilePath)).ToList();

                changelogFileLines.Insert(2, changelogEntry);

                await File.WriteAllLinesAsync(moduleChangelogFilePath, changelogFileLines);
            }
            else
            {
                await File.WriteAllLinesAsync(moduleChangelogFilePath, [$"# {moduleChangelog.ModuleName}", string.Empty, changelogEntry]);
            }
        }
    }

    private static string GenerateChangelogFile(ModuleChangelog moduleChangelog)
    {
        string newVersion = moduleChangelog.GetNewVersion().ToString();

        var groupedDescriptions = moduleChangelog.Changes
            .GroupBy(change => change.BumpType)
            .Select(bumpToChangesGroup => new
            {
                BumpType = bumpToChangesGroup.Key,
                Descriptions = bumpToChangesGroup.Select(change => change.Description)
            })
            .OrderByDescending(bumpWithChanges => (int)bumpWithChanges.BumpType);

        return
        $"""
        ## {newVersion}
        {string.Join("",
                groupedDescriptions.Select(groupedDesc => GenerateChangelogBumpTypeSection(groupedDesc.BumpType, groupedDesc.Descriptions))
            )}
        """;
    }

    private static string GenerateChangelogBumpTypeSection(BumpType bumpType, IEnumerable<string> descriptions)
    {
        return
       $"""

       **{bumpType} Changes**:

       {string.Join(Environment.NewLine, descriptions.Select(description => $"- {description}"))}

       """;
    }
}

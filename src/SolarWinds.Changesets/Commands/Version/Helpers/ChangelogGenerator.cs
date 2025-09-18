using System.Collections.Immutable;
using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

internal sealed class ChangelogGenerator : IChangelogGenerator
{
    /// <inheritdoc/>
    public IEnumerable<ModuleChangelog> GetProcessedChangelogs(ChangesetFile[] changesets, IEnumerable<CsProject> csProjects)
    {
        List<ModuleChangelog> moduleChangelogs = GenerateModuleChangelogs(changesets, csProjects);

        ImmutableDictionary<string, CsProject[]> csProjectToDependentProjects = GetProjectsDependentsNames(csProjects);

        List<ModuleChangelog> dependentModulesChangelogs = [];

        foreach (ModuleChangelog module in moduleChangelogs)
        {
            IEnumerable<ModuleChangelog> changelogs = GetDependentModulesChangelogs(module, csProjectToDependentProjects);
            dependentModulesChangelogs.AddRange(changelogs);
        }

        IEnumerable<ModuleChangelog> allChangelogs = moduleChangelogs.Concat(dependentModulesChangelogs);

        IEnumerable<ModuleChangelog> finalGroupedChangelogs = GroupModuleChangelogs(allChangelogs);

        return finalGroupedChangelogs;
    }

    /// <summary>
    /// Processes changeset files and filters out unique modules with their <see cref="ModuleChangelog"/>
    /// </summary>
    /// <param name="changesetFiles"></param>
    /// <param name="csProjects"></param>
    /// <returns>Collection of <see cref="ModuleChangelog"/></returns>
    private static List<ModuleChangelog> GenerateModuleChangelogs(
        ChangesetFile[] changesetFiles, IEnumerable<CsProject> csProjects)
    {
        Dictionary<string, ModuleChangelog> moduleChangelogs = [];

        foreach (ChangesetFile changesetFile in changesetFiles)
        {
            foreach (string changedModuleName in changesetFile.ChangedModuleNames)
            {
                if (!moduleChangelogs.TryGetValue(changedModuleName, out ModuleChangelog? moduleChangelog))
                {
                    CsProject? changedModuleCsProject = csProjects.SingleOrDefault(p => p.Name == changedModuleName);
                    if (changedModuleCsProject is null)
                    {
                        //Console.WriteLine("Something is wrong");
                        continue;
                    }

                    moduleChangelog = new ModuleChangelog
                    {
                        ModuleName = changedModuleCsProject.Name,
                        ModuleCsProjFilePath = changedModuleCsProject.CsprojFilePath,
                        CurrentVersion = changedModuleCsProject.Version
                    };
                    moduleChangelogs[changedModuleName] = moduleChangelog;
                }

                moduleChangelog.Changes.Add((changesetFile.Description, changesetFile.BumpType));
            }
        }

        return moduleChangelogs.Values.ToList();
    }

    /// <summary>
    /// Determine projects which reference particular ProjectReference.
    /// </summary>
    /// <param name="csProjects"></param>
    /// <returns>Dictionary, where key is ProjectReference and value is unique list of projects dependent on the ProjectReference</returns>
    private static ImmutableDictionary<string, CsProject[]> GetProjectsDependentsNames(IEnumerable<CsProject> csProjects)
    {
        Dictionary<string, HashSet<CsProject>> csProjectNameToDependentProjects = [];

        foreach (CsProject csProject in csProjects)
        {
            foreach (string projectReferenceName in csProject.ReferencedProjectNames)
            {
                if (csProjectNameToDependentProjects.TryGetValue(projectReferenceName, out HashSet<CsProject>? value))
                {
                    value.Add(csProject);
                }

                csProjectNameToDependentProjects[projectReferenceName] = [csProject];
            }
        }

        return csProjectNameToDependentProjects.ToImmutableDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moduleChangelog"></param>
    /// <param name="csProjectToDependentProjects"></param>
    /// <returns></returns>
    private static List<ModuleChangelog> GetDependentModulesChangelogs(
       ModuleChangelog moduleChangelog,
       ImmutableDictionary<string, CsProject[]> csProjectToDependentProjects
       )
    {
        List<ModuleChangelog> dependentModulesChangelogs = [];

        CsProject[] dependentProjects = GetDependentProjects(moduleChangelog.ModuleName, csProjectToDependentProjects);

        foreach (CsProject dependentProject in dependentProjects)
        {
            ModuleChangelog dependentModuleChangelog = new()
            {
                ModuleName = dependentProject.Name,
                ModuleCsProjFilePath = dependentProject.CsprojFilePath,
                CurrentVersion = dependentProject.Version,
            };
            dependentModuleChangelog.Changes.Add(
                new($"Updated dependencies: {moduleChangelog.ModuleName}:{moduleChangelog.GetNewVersion()}", BumpType.Patch) // TODO format Description
            );
            dependentModulesChangelogs.Add(dependentModuleChangelog);
            IEnumerable<ModuleChangelog> recursive = GetDependentModulesChangelogs(dependentModuleChangelog, csProjectToDependentProjects);
            dependentModulesChangelogs.AddRange(recursive);
        }

        return GroupModuleChangelogs(dependentModulesChangelogs);
    }

    private static List<ModuleChangelog> GroupModuleChangelogs(IEnumerable<ModuleChangelog> moduleChangelogs)
    {
        return moduleChangelogs
            .GroupBy(m => m.ModuleName)
            .Select(group => new ModuleChangelog
            {
                ModuleName = group.Key,
                ModuleCsProjFilePath = group.First().ModuleCsProjFilePath,
                CurrentVersion = group.First().CurrentVersion, // Assume the version is the same for grouped items
                Changes = group.SelectMany(g => g.Changes).ToList()
            })
            .ToList();
    }

    private static CsProject[] GetDependentProjects(string projectName, ImmutableDictionary<string, CsProject[]> csProjectToDependentProjects)
    {
        if (csProjectToDependentProjects.TryGetValue(projectName, out CsProject[]? dependents))
        {
            return dependents;
        }

        return [];
    }
}

using System.Collections.Immutable;
using SolarWinds.Changesets.Shared;

namespace SolarWinds.Changesets.Commands.Version.Helpers;

internal sealed class ChangelogGenerator : IChangelogGenerator
{
    /// <inheritdoc/>
    public IEnumerable<ModuleChangelog> GetProcessedChangelogs(ChangesetFile[] changesets, IEnumerable<DotnetProject> projects)
    {
        List<ModuleChangelog> moduleChangelogs = GenerateModuleChangelogs(changesets, projects);

        ImmutableDictionary<string, DotnetProject[]> projectToDependentProjects = GetProjectsDependentsNames(projects);

        List<ModuleChangelog> dependentModulesChangelogs = [];

        foreach (ModuleChangelog module in moduleChangelogs)
        {
            IEnumerable<ModuleChangelog> changelogs = GetDependentModulesChangelogs(module, projectToDependentProjects);
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
    /// <param name="projects"></param>
    /// <returns>Collection of <see cref="ModuleChangelog"/></returns>
    private static List<ModuleChangelog> GenerateModuleChangelogs(
        ChangesetFile[] changesetFiles, IEnumerable<DotnetProject> projects)
    {
        Dictionary<string, ModuleChangelog> moduleChangelogs = [];

        foreach (ChangesetFile changesetFile in changesetFiles)
        {
            foreach (string changedModuleName in changesetFile.ChangedModuleNames)
            {
                if (!moduleChangelogs.TryGetValue(changedModuleName, out ModuleChangelog? moduleChangelog))
                {
                    DotnetProject? changedModuleProject = projects.SingleOrDefault(p => p.Name == changedModuleName);
                    if (changedModuleProject is null)
                    {
                        //Console.WriteLine("Something is wrong");
                        continue;
                    }

                    moduleChangelog = new ModuleChangelog
                    {
                        ModuleName = changedModuleProject.Name,
                        ModuleProjectFilePath = changedModuleProject.ProjectFilePath,
                        CurrentVersion = changedModuleProject.Version
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
    /// <param name="projects"></param>
    /// <returns>Dictionary, where key is ProjectReference and value is unique list of projects dependent on the ProjectReference</returns>
    private static ImmutableDictionary<string, DotnetProject[]> GetProjectsDependentsNames(IEnumerable<DotnetProject> projects)
    {
        Dictionary<string, HashSet<DotnetProject>> projectNameToDependentProjects = [];

        foreach (DotnetProject project in projects)
        {
            foreach (string projectReferenceName in project.ReferencedProjectNames)
            {
                if (projectNameToDependentProjects.TryGetValue(projectReferenceName, out HashSet<DotnetProject>? value))
                {
                    value.Add(project);
                }

                projectNameToDependentProjects[projectReferenceName] = [project];
            }
        }

        return projectNameToDependentProjects.ToImmutableDictionary(pair => pair.Key, pair => pair.Value.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moduleChangelog"></param>
    /// <param name="projectToDependentProjects"></param>
    /// <returns></returns>
    private static List<ModuleChangelog> GetDependentModulesChangelogs(
       ModuleChangelog moduleChangelog,
       ImmutableDictionary<string, DotnetProject[]> projectToDependentProjects
       )
    {
        List<ModuleChangelog> dependentModulesChangelogs = [];

        DotnetProject[] dependentProjects = GetDependentProjects(moduleChangelog.ModuleName, projectToDependentProjects);

        foreach (DotnetProject dependentProject in dependentProjects)
        {
            ModuleChangelog dependentModuleChangelog = new()
            {
                ModuleName = dependentProject.Name,
                ModuleProjectFilePath = dependentProject.ProjectFilePath,
                CurrentVersion = dependentProject.Version,
            };
            dependentModuleChangelog.Changes.Add(
                new($"Updated dependencies: {moduleChangelog.ModuleName}:{moduleChangelog.GetNewVersion()}", BumpType.Patch) // TODO format Description
            );
            dependentModulesChangelogs.Add(dependentModuleChangelog);
            IEnumerable<ModuleChangelog> recursive = GetDependentModulesChangelogs(dependentModuleChangelog, projectToDependentProjects);
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
                ModuleProjectFilePath = group.First().ModuleProjectFilePath,
                CurrentVersion = group.First().CurrentVersion, // Assume the version is the same for grouped items
                Changes = group.SelectMany(g => g.Changes).ToList()
            })
            .ToList();
    }

    private static DotnetProject[] GetDependentProjects(string projectName, ImmutableDictionary<string, DotnetProject[]> projectToDependentProjects)
    {
        if (projectToDependentProjects.TryGetValue(projectName, out DotnetProject[]? dependents))
        {
            return dependents;
        }

        return [];
    }
}

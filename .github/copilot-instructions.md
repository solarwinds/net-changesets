# Copilot Instructions for NET Changesets

This guide enables AI coding agents to work productively in the NET Changesets codebase.

## Repository purpose

NET Changesets is a .NET CLI tool for managing versioning and changelogs in multi-package repositories, inspired by `@changesets/cli` from the npm ecosystem. It provides unified `CHANGELOG.md` generation, semantic versioning across project dependencies, and automated NuGet package publishing. The tool serves .NET developers working in monorepo or multi-project solutions who need coordinated versioning and release management. Current stage: MVP with manual CLI operations; GitHub Action automation planned for future release.

## Technology stack

**Language:** C# 12.0 with nullable reference types and implicit usings enabled  
**Runtime:** .NET 8.0 (SDK 8.0.406)  
**CLI Framework:** Spectre.Console v0.50.0 (interactive prompts, tables, markup rendering)  
**Dependency Injection:** Microsoft.Extensions.DependencyInjection v9.0.9  
**Testing:** NUnit 4.4.0, Moq 4.20.72, AwesomeAssertions 9.1.0, Spectre.Console.Testing  
**Code Analysis:** Microsoft .NET analyzers (all enabled, warnings-as-errors), Spectre.Console.Analyzer  
**Package Management:** Central Package Management via `Directory.Packages.props`  
**CI/CD:** GitHub Actions (build, test, pack, format verification)  
**External Tools:** Git CLI (for diff detection), dotnet CLI (for pack/publish)

## Directory structure

```
SolarWinds.Changesets.sln          # Main solution file
├── src/SolarWinds.Changesets/     # CLI tool source code
│   ├── Program.cs                 # Entry point: service registration + Spectre.Console CommandApp configuration
│   ├── Commands/                  # Each command in own folder (Add, Init, Publish, Status, Version)
│   ├── Services/                  # GitService, DotnetService (wrapping external process execution)
│   ├── Shared/                    # Models: Semver, ChangesetConfig, ChangesetFile, CsProject, ProcessExecutor
│   └── Infrastructure/            # TypeRegistrar, TypeResolver (DI integration with Spectre.Console)
├── tests/SolarWinds.Changesets.Tests/  # Test project
│   ├── Add/, Init/, Publish/, Status/, Version/  # Tests organized by command
│   └── TestData/                  # Test fixtures (sample .csproj files, changesets)
├── docs/
│   ├── commands-implementation-details.md  # Command behavior differences from npm version
│   └── config-file-options.md     # .changeset/config.json schema
├── Directory.Build.props          # Shared MSBuild properties: TargetFramework, LangVersion, code analysis rules
├── Directory.Packages.props       # Central package version management
└── global.json                    # .NET SDK version pinning
```

## Architecture

**CLI Framework:** Uses Spectre.Console.Cli with `CommandApp<AddChangesetCommand>` (Add is default). Commands registered in `Program.cs` via `config.AddCommand<T>()`. All commands inherit from `ConfigurationCommandBase` which loads `.changeset/config.json`.

**Dependency Injection:** Services registered in `ServiceCollection` and integrated via custom `TypeRegistrar`/`TypeResolver` (required by Spectre.Console.Cli). Main services:
- `IConfigurationService`: Loads/validates `.changeset/config.json`
- `IChangesetsRepository`: Reads/writes/deletes changeset markdown files
- `ICsProjectsRepository`: Parses .csproj files, updates `<VersionPrefix>` elements
- `IChangelogGenerator`/`IChangelogFileWriter`: Generates `CHANGELOG.md` from changesets
- `IGitService`, `IDotnetService`: Wrap `git` and `dotnet` CLI via `IProcessExecutor`

**Process Execution:** `ProcessExecutor` class wraps `System.Diagnostics.Process` to execute external commands (git, dotnet) with stdout/stderr capture. Services depend on this abstraction for testability.

**Semantic Versioning:** Custom `Semver` class (Major.Minor.Patch) with methods `RaiseMajor()`, `RaiseMinor()`, `RaisePatch()`. Parses version strings from `<VersionPrefix>` in .csproj files. Version bumps cascade dependencies (updating dependent projects).

**Command Flow Example (version command):**
1. `VersionChangesetCommand.ExecuteCommandAsync()` calls `IChangesetsRepository.GetChangesetsAsync()`
2. Reads all `.md` files from `.changeset/` directory
3. Parses YAML front matter (project names, bump types) and markdown content
4. `ICsProjectsRepository.GetCsProjects()` loads all .csproj from `sourcePath` (default: `src`)
5. `IChangelogGenerator.GetProcessedChangelogs()` calculates new versions + dependency updates
6. `IChangelogFileWriter.GenerateChangelogFilesAsync()` writes `CHANGELOG.md`
7. `ICsProjectsRepository.UpdateCsProjectsVersionAsync()` updates `<VersionPrefix>` in .csproj files
8. `IChangesetsRepository.DeleteChangesets()` removes processed changeset files

## Build instructions

**Prerequisites:** .NET 8.0 SDK (version 8.0.406 or compatible via rollForward in `global.json`)

**Build:**
```bash
dotnet restore --packages ./packages
dotnet build -c Release --no-restore
```

**Test:**
```bash
dotnet test -c Release --no-restore --no-build --verbosity normal
```

**Pack:**
```bash
dotnet pack -c Release --no-restore --no-build
# Output: ./nupkg/SolarWinds.Changesets.0.1.1.nupkg
```

**Code Style Check:**
```bash
dotnet format --no-restore --verify-no-changes
```

**Local Installation for Testing:**
```bash
dotnet tool uninstall solarwinds.changesets --global
dotnet tool install solarwinds.changesets --global --add-source ./nupkg
# TODO: this is not yet implemented, see issue #12
# changeset --version
```

**CI Pipeline:** `.github/workflows/ci.yml` runs on PR/push to main. Steps: checkout, restore, build, test, pack, upload artifact, format verification. Uses ubuntu-latest runner.

## Manual testing and verification

After implementing changes, verify the behavior by running the tool directly from source:

```bash
dotnet run --project .\src\SolarWinds.Changesets\SolarWinds.Changesets.csproj
```

**Verification checklist:**
1. ✅ Build succeeds: `dotnet build -c Release --no-restore`
2. ✅ All tests pass: `dotnet test -c Release --no-restore --no-build`
3. ✅ Code formatting: `dotnet format --no-restore --verify-no-changes`
4. ✅ No compilation errors: Check with IDE or `dotnet build`
5. ✅ Manual dry run: Test actual behavior with `dotnet run`

## Domain configurations

**Changeset Config (`.changeset/config.json`):**
```json
{
    "sourcePath": "src",        // Relative path to projects folder
    "packageSource": "nuget"    // NuGet source name from NuGet.config
}
```
Created by `changeset init`. Loaded by `ConfigurationService`. Default `packageSource` is `nuget.org`.

**NuGet.config:** Optional file for custom package sources. Example:
```xml
<packageSources>
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
    <add key="NugetFolderSource" value=".\NugetFolderSource" />
</packageSources>
```

**Changeset File Format (`.changeset/{randomname}.md`):**
```markdown
---
"ProjectA": minor
"ProjectB": patch
---

Added new feature X and fixed bug Y in ProjectB
```
Generated by `add` command. Filename: 10 random lowercase letters + `.md`. Projects listed in YAML front matter with bump type (major/minor/patch).

**Project File Requirements:**
- Must contain `<VersionPrefix>` element (e.g., `<VersionPrefix>1.0.0</VersionPrefix>`)
- Supported format: `Major.Minor.Patch` (parsed via `System.Version`)
- `version` command updates this element in-place using XML manipulation

## Conventions

**Code Style:**
- **Implicit usings enabled** (no need for common System namespaces)
- **Nullable reference types enabled** (treat null warnings as errors)
- **File-scoped namespaces** (e.g., `namespace SolarWinds.Changesets.Commands.Add;`)
- **XML documentation required** for public APIs (`GenerateDocumentationFile=true`)
- **Code analysis:** All .NET analyzers enabled (`AnalysisMode=all`), warnings treated as errors
- **IDE rules enforced in CI** via `dotnet format` (not in build)

**Testing:**
- NUnit framework with `[TestFixture]` and `[Test]` attributes
- Global using for `NUnit.Framework` (defined in test .csproj)
- Test data in `TestData/` folders, copied to output directory
- Use `Spectre.Console.Testing.TestConsole` for command output assertions
- Mock external processes using `Moq` on `IProcessExecutor`

**Naming:**
- Commands: `{Action}ChangesetCommand` (e.g., `AddChangesetCommand`)
- Interfaces: Standard `I` prefix (e.g., `IConfigurationService`)
- Internal classes: Most implementation classes are `internal sealed`
- Constants: Defined in `Constants` static class (e.g., `Constants.WorkingDirectoryFullPath`)

**Dependency Constraints:**
- **Only allowed third-party dependency:** Spectre.Console (and related packages)
- Rationale: Minimize external dependencies for a CLI tool
- All other needs met by .NET BCL or Microsoft.Extensions packages

**Git Workflow:**
- Branch naming: `{type}/{issueID}-{description}` (e.g., `feature/123-add-new-command`)
- Commit messages: Start with issue ID (e.g., `123 Implement status command`)
- GPG signing required for commits
- PR title format: `{Type} #{issueID} {Description}`

**Error Handling:**
- `ExceptionHandler.Handle()` registered in Spectre.Console CLI config
- Custom exception: `InitializationException` for config validation errors
- Return codes defined in `ResultCodes` class (not yet fully implemented)

## Integration points

**Git Integration:**
- `GitService.GetDiff()` calls `git diff --name-only {sourcePath}` to detect modified .csproj files
- Used by `publish` command to identify packages needing publication
- Requires git executable in PATH

**Dotnet CLI Integration:**
- `DotnetService.Pack()` calls `dotnet pack {projectPath} --output {Constants.NupkgOutputFullPath}`
- `DotnetService.Publish()` calls `dotnet nuget push {nupkgPath} --source {packageSource}`
- NuGet API key expected in environment or nuget.config (standard dotnet behavior)
- Output directory: `./nupkg/` (created if not exists)

**File System Operations:**
- Changeset files: `.changeset/` directory (created by `init` command)
- CHANGELOG.md: Root of repository
- .csproj files: Located via recursive search from `sourcePath` config
- All paths resolved relative to `Constants.WorkingDirectoryFullPath` (current directory)

**NuGet Sources:**
- Resolved via standard dotnet/NuGet.config mechanisms
- Default: `nuget` source (typically nuget.org)
- Custom sources defined in `NuGet.config` (repository or user-level)
- Folder sources recommended for local testing (e.g., `.\NugetFolderSource`)

## Project status & roadmap

**Current State:** MVP stage with manual CLI operations. All 5 commands implemented (`init`, `add`, `version`, `publish`, `status`).

**Known Limitations:**
- `add` command supports only single bump type for all selected projects (npm version allows per-project bumps)
- No command-line options for `add` (interactive mode only)
- Manual execution required (no GitHub Action yet)

**Planned Features:**
- **GitHub Action** (primary roadmap item): Automated PR creation for version bumps, reusing existing codebase. See `.NET GitHub Action` documentation.
- Future improvements open for discussion via GitHub issues

## References

- **Commands:** `docs/commands-implementation-details.md` (npm differences)
- **Configuration:** `docs/config-file-options.md` (schema details)
- **Contributing:** `CONTRIBUTING.md` (PR process, code style, local testing)
- **Roadmap:** `ROADMAP.md` (GitHub Action plans)
- **Original inspiration:** [@changesets/cli](https://github.com/changesets/changesets) (npm package)

---

**Feedback:** If any section is unclear or missing, please specify so it can be improved for future AI agents.

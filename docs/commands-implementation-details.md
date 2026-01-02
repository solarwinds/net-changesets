# Commands implementation details

On this page, we will discuss implementation details that differ slightly from the original changeset implementation [@changesets/cli](https://github.com/changesets/changesets/tree/main).

## `init`

```bash
changeset init
```

This command works the same, only difference is that we have different configuration schema to satisfy .NET implementation. You can read more about it in [config-file-options.md](https://github.com/solarwinds/net-changesets/blob/main/docs/config-file-options.md).

## `add`

```bash
changeset add
```

or just (`add` is the default command)

```bash
changeset
```

This is the default command when running Changesets without specifying a command.
Currently, the tool does not support any command-line options and runs only in interactive mode.
The changeset file name is randomly generated using lowercase letters from the English alphabet.

How it works

- The command reads all .csproj project files from the SourcePath.
- It prompts the user with a multi-select option to choose which projects have changed.
- It prompts the user to select the type of change (patch, minor, or major).
- It prompts the user to describe what has changed.
- It generates a changeset file with the selected projects, change type, and description and stores it in `.changeset` folder.

Notes
The only difference compared to the Node.js implementation is that this version currently supports selecting only a single change type for all selected projects.
The Node.js version allows selecting a different change type for each project individually.
This additional functionality can be implemented easily if needed.

## `version`

```bash
changeset version
```

If there are any existing changesets the command generates changelogs for every project dependent on the updated project. It also amends SemVer version of the affected project.

## `publish`

```bash
changeset publish
```

Creates NuGet packages of affected projects and publishes them to the predefined package source.

### How it works (current .NET implementation)

Currently, the `publish` command assumes that the changes made by the `version` command have **already been committed**. The intended flow is:

1. Run `changeset version` to bump versions, update changelogs, and delete processed changesets.
2. Commit the changes produced by the `version` command.
3. Run `changeset publish` to create and push NuGet packages based on the committed changes.

The `publish` command does following:

1. Gets all modified `.csproj` files from the predefined source directory by comparing the last two commits `git diff --name-only HEAD~1 HEAD {sourcePath}`
2. For each changed `.csproj` file, creates a NuGet package using `dotnet pack`.
3. Pushes the created NuGet packages to the predefined NuGet source using `dotnet nuget push`.

The package source can be configured via the `.changeset/config.json` file (see `docs/config-file-options.md`).

This approach relies on the fact that the `version` command only performs three types of changes:

- Deleting processed changeset files from the `.changeset` folder
- Modifying or creating `CHANGELOG.md` files
- Bumping versions in `.csproj` files

By looking at the diff between `HEAD~1` and `HEAD`, `publish` can safely identify which projects had their versions bumped and therefore need to be published.

### Comparison with original Node.js changesets implementation

The original `@changesets/cli` implementation for Node.js works differently. Instead of relying on a Git diff,
it checks whether a package with the **current version** from `package.json` already exists in the package registry; if it does not, the package is published.

## `status`

```bash
changeset status
```

The command writes number of changesets in the `.changeset` folder to the console.

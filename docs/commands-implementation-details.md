# Commands implementation details

On this page, we will discuss implementation details that differ slightly from the original changeset implementation [@changesets/cli](https://github.com/changesets/changesets/tree/main).

## `init`

```bash
changeset init
```

This command works the same, only difference is that we have different configuration schema to satisfy .NET implementation. You can read more about it in [config-file-options.md](./config-file-options.md).

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

Creates nuget packages of affected projects and publishes them to the predefined package source. The command:

1. Gets all modified `.csproj` files from the predefined working directory using `git diff --name-only`.
1. Creates nuget package using `nuget pack` for every csproj file.
1. Pushes the created nuget packages to the predefined nuget source using `nuget push`.

The package source can be configured via `config.json` file.

## `status`

```bash
changeset status
```

The command writes number of changesets in the `.changeset` folder to the console.

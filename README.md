# NET Changesets

[![NuGet version (SolarWinds.Changesets)](https://img.shields.io/nuget/v/SolarWinds.Changesets.svg)](https://www.nuget.org/packages/SolarWinds.Changesets)

A .NET CLI tool (with interactive support) to manage versioning and changelogs with a focus on multi-package repositories.

Key features include:

- Generates `CHANGELOG.md` in unified form
- Calculates new version of module based on:
  - changes in current module
  - changes in all dependent modules
- Publishes packages into NuGet artifactory

This .NET implementation is port from original `npm` implementation [@changesets/cli](https://www.npmjs.com/package/@changesets/cli)
([GitHub repository](https://github.com/changesets/changesets/blob/main/README.md)).

## CLI

**Usage:**

`changeset [OPTIONS] [COMMAND]`

**Options:**

- `-h, --help` Prints help information
- `-v, --version` Prints version information

**Commands:**

- `init` Sets up the .changeset folder and generates a default config file. You should run this command once when you are setting up changesets
- `add` Used by contributors to add information about their changes by creating changeset files
- `version` Takes existing changeset files and updates versions and dependencies of packages, as well as writing changelogs
- `publish` This publishes changes to specified nuget repository
- `status` Provides information about the changesets that currently exist. If there are no changesets present, it exits with an error status code

## Documentation

- [Implementation Details of net-changesets commands](https://github.com/solarwinds/net-changesets/blob/main/docs/commands-implementation-details.md)
- [Config file options](https://github.com/solarwinds/net-changesets/blob/main/docs/config-file-options.md)

## Roadmap

You can find the **Roadmap** [here](https://github.com/solarwinds/net-changesets/blob/main/ROADMAP.md).

## Contributing

You can find the **Contribution Guidelines** [here](https://github.com/solarwinds/net-changesets/blob/main/CONTRIBUTING.md).

## Installation

Install tool by running [dotnet tool install](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install):

```bash
dotnet tool install solarwinds.changesets --global
```

Update tool by running [dotnet tool update](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-update):

```bash
dotnet tool update solarwinds.changesets --global
```

Uninstall tool by running [dotnet tool uninstall](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-uninstall):

```bash
dotnet tool uninstall solarwinds.changesets --global
```

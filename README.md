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

## Changesets GitHub Action Integration with the .NET Changesets Implementation

net-changesets can be used together with the original [changesets GitHub action](https://github.com/changesets/action). The action supports custom commands for the versioning and publishing steps, so a net-changesets tool can be used instead of the default [JavaScript-based changesets CLI tool](https://github.com/changesets/changesets).

After each merge to the main branch, the action creates a release PR. During that step, `changeset version` converts changeset files into changelog entries and bumps C# project versions. When the release PR is merged, `changeset publish` builds and publishes NuGet packages to the configured nuget source.

Minimal action configuration:

```yaml
- name: Create release pull request or publish packages
  uses: changesets/action@v1
  with:
    version: changeset version
    publish: changeset publish
```
Just a note: we don't need to override changesets status, because both the .NET and Node.js implementations use the same behavior for this command.

Important workflow details:

- Checkout should use `fetch-depth: 2`, because `changeset publish` compares the last two commits to find changed packages.
- If the workflow runs `changeset status`, use `continue-on-error: true` when an empty changeset state should not fail the job.
- The publish target is controlled by the net-changesets configuration and NuGet configuration. The demo publishes to a local folder package source configured in [.changeset/config.json](https://github.com/magiino/Marekth.NetChangesets.Demo/blob/main/.changeset/config.json#L3) and [nuget.config](https://github.com/magiino/Marekth.NetChangesets.Demo/blob/main/nuget.config).

For a complete working example, see the [net-changesets demo repository](https://github.com/magiino/Marekth.NetChangesets.Demo/tree/main), especially the configured [release workflow](https://github.com/magiino/Marekth.NetChangesets.Demo/blob/main/.github/workflows/release.yml). The workflow wires the changesets action to net-changesets through its custom command inputs, as shown around [line 44](https://github.com/magiino/Marekth.NetChangesets.Demo/blob/main/.github/workflows/release.yml#L44). It also shows the required checkout depth at [line 16](https://github.com/magiino/Marekth.NetChangesets.Demo/blob/main/.github/workflows/release.yml#L16) and a non-failing status check at [line 48](https://github.com/magiino/Marekth.NetChangesets.Demo/blob/main/.github/workflows/release.yml#L48). An example generated release PR is available at [magiino/Marekth.NetChangesets.Demo#7](https://github.com/magiino/Marekth.NetChangesets.Demo/pull/7), and the repository [commit history](https://github.com/magiino/Marekth.NetChangesets.Demo/commits/main/) shows the release flow in detail.

**You can fork the demo repository and try the workflow yourself !!! :)**

The changesets action can also customize the release PR title, description, commit message, and other behavior. See the action [inputs documentation](https://github.com/changesets/action?tab=readme-ov-file#inputs) for the full list of supported options.

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

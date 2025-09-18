# NET Changesets

A .NET CLI tool (with interactive support) to manage versioning and changelogs with a focus on multi-package repositories.

Key features include:

- Generates `CHANGELOG.md` in unified form
- Calculates new version of module based on:
  - changes in current module
  - changes in all dependent modules
- Publishes packages into NuGet artifactory

This .NET implementation is port from original `npm` implementation [@changesets/cli](https://www.npmjs.com/package/@changesets/cli)
([GitHub repository](https://github.com/changesets/changesets/blob/main/README.md)).

## Currently Supported Commands

- `changeset init`
- `changeset add`
- `changeset version`
- `changeset publish`
- `changeset status`

## Documentation

- [Implementation Details of net-changesets commands](./docs/commands-implementation-details.md)
- [Config file options](./docs/config-file-options.md)

## Roadmap

You can find the **Roadmap** [here](./ROADMAP.md).

## Contributing

You can find the **Contribution Guidelines** [here](./CONTRIBUTING.md).

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

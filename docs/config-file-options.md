# Configuring Changesets

Changesets has a minimal amount of configuration options. Mostly these are for when you need to change the default workflows. These are stored in `.changeset/config.json`. The default config is:

```json
{
    "sourcePath": "src",
    "packageSource": "nuget"
}
```

## `sourcePath`

Specify the relative path from the changeset command's execution folder to the location of the projects.

The default value is `src`.

## `packageSource`

Specify the NuGet package source where the packages will be published.

Default value is `nuget`. It is official [nuget.org](https://www.nuget.org/) repository.

You can define NuGet package sources using `nuget.config`. To create a default `nuget.config`, use the following CLI command:

```bash
dotnet new nugetconfig
```

This allows you to specify your own NuGet package sources. For example, you can define the `.\NugetFolderSource` folder as a source.

Using folder sources is the best way to verify the output of your changesets setup!

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <add key="NugetFolderSource" value=".\NugetFolderSource" />
  </packageSources>
</configuration>
```

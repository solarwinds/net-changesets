# SolarWinds.Changesets

## 0.1.3

**Patch Changes**:

- [[#6](https://github.com/solarwinds/net-changesets/issues/6)] Add Contributor Covenant Code of Conduct ([PR #14](https://github.com/solarwinds/net-changesets/pull/14))
- [[#12](https://github.com/solarwinds/net-changesets/issues/12)] Add `-v, --version` option to changeset CLI and other small fixes ([PR #15](https://github.com/solarwinds/net-changesets/pull/15))
  - Replace package icon with transparent one
  - Fix `MD012/no-multiple-blanks` linter warning in generated changelog that was caused by two new lines at the bottom of a generated section instead of one
  - Fix `MD024/no-duplicate-heading` linter warning in generated changelog that was caused by duplicate headings. This `### {bumpType} Changes` was replaced with this `**{bumpType} Changes**:`
  - Use absolute links to Markdown documents instead of relative ones, which may not work in some contexts (e.g., NuGet package page)
  - `TreatWarningsAsErrors` is changing code style warnings to errors, but we want to keep them as warnings
  - Remove leftover code for 'not initialized' error message and improve current 'not initialized' error message
  - Fix formatting of JSON, YML, XML and Markdown files to use 2-space indentation

## 0.1.2

**Patch Changes**:

- [NO-ISSUE] Add security info file ([PR #5](https://github.com/solarwinds/net-changesets/pull/5))
- [[#9](https://github.com/solarwinds/net-changesets/issues/9)] Copilot instructions and minor improvements for first start ([PR #10](https://github.com/solarwinds/net-changesets/pull/10))
- [[#8](https://github.com/solarwinds/net-changesets/issues/8)] Improved error handling when the changeset is not initialized. ([PR #11](https://github.com/solarwinds/net-changesets/pull/11))

## 0.1.1

**Patch Changes**:

- [[#3](https://github.com/solarwinds/net-changesets/issues/3)] Add missing nuget package metadata ([PR #4](https://github.com/solarwinds/net-changesets/pull/4))

## 0.1.0

**Minor Changes**:

- [[#1](https://github.com/solarwinds/net-changesets/issues/1)] Initial release of .NET Changesets CLI tool ([PR #2](https://github.com/solarwinds/net-changesets/pull/2))
  - USAGE: `changeset [OPTIONS] [COMMAND]`
  - Currently Supported Options:
    - `-h, --help`
  - Currently Supported Commands:
    - `init`
    - `add`
    - `version`
    - `publish`
    - `status`

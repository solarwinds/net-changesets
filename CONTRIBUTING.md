# How to contribute

We do very welcome everyone who wants to contribute to this repository, provided guidelines summarized in this file are followed.

## Ask first

Please **discuss any changes you wish to make** with the net-changesets maintainers via GitHub issues before proceeding.

If you don’t have a specific idea in mind, you can pick an issue from the [roadmap](https://github.com/solarwinds/net-changesets/blob/main/ROADMAP.md). Your help is greatly appreciated!

## Design considerations

Always consider the following aspects:

- Performance impacts
- Security impacts
- Maintenance, extensibility, and code readability
- Breaking changes
- How to test your feature using unit tests or integration tests
- Do not introduce any third-party dependencies. Currently, the only allowed third-party dependency is `Spectre.Console`.

## Development

### Set up your environment

- You must setup gpg key for sign commits. Checkout the official [GitHub manual](https://docs.github.com/en/authentication/managing-commit-signature-verification).
- **Create a fork** of the `net-changesets` repository under your GitHub account.
- **Create a branch** with a descriptive name that includes the change type (feature, bugfix, etc.), the GitHub issue ID, and a title, all connected with dashes. Example: `feature/123-Some-meaningful-name`
- **Organize your changes** into logically grouped and ordered commits (If the feature is large, this helps reviewers)
  - Each commit message must start with the GitHub issue ID. Example: `123 Some meaningful name`
- **Do not make any additional changes unrelated to your GitHub issue.**
- **Every change must be covered by a unit or integration test.**
- **Write clean, self-documenting code** and add XML comments to newly added code.
- **Edit or add documentation** in the `docs` folder or in [README.md](https://github.com/solarwinds/net-changesets/blob/main/README.md).

#### Local Installation

Please note, `dotnet pack` MSBuild properties like `PackageID`, `Version` and others are defined in `.csproj` and `Directory.Build.props` files.

Please fill the version from csproj in curly braces `{VersionFromCsproj}`.

```bash
dotnet pack --version-suffix Alpha.1
dotnet tool uninstall solarwinds.changesets --global
dotnet tool install solarwinds.changesets --global --add-source ./nupkg --version {VersionFromCsproj}-Alpha.1
```

### Code Style and Code Quality

This project uses only first-party [.NET source code analysis](https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/overview?tabs=net-9) from Microsoft.

The [.editorconfig](https://github.com/solarwinds/net-changesets/blob/main/.editorconfig) and [Directory.Build.props](https://github.com/solarwinds/net-changesets/blob/main/Directory.Build.props) files contain the configuration for code style and quality.

- **Code style (`IDE****`) analyzers are disabled during build time.**
  - They are checked in PR CI as the last step.
- **Code quality rules (`CA****`, `CS****`, and others) affect the build and must be respected.**

If you need to enable or disable a code style or code quality analyzer, you may do so—but you must explain and defend your decision in the pull request.

## Pull Request

Create a pull request only if:

- **You have ensured that the code compiles and all tests pass on your development environment.**
- **You have checked code style and quality using the `dotnet format` command before creating the pull request.**
- **You believe that all your code changes are ready.** This means the scope of the task is achieved, there are no bugs, and code quality is preserved.

### Pull Request Guidelines

- **Title**
  - Use the branch name as a guide for the PR title. Example:
    - Branch name: `feature/123-Some-meaningful-name`
    - PR title: `Feature #123 Some meaningful name`
- **Body**
  - Follow the pull request template and fill in all required details.
  - The description is especially important.
    - Include all information that will help reviewers understand the changes.
    - Explain what you tried, what did not work, what the changes affect, and what the changes mean.
    - Sometimes a single line change can have a big impact that is not obvious from the PR. A brief description helps reviewers understand your changes more quickly.
  - Do not hesitate to add your own comments to describe your decisions and reasoning.

### Review

- **Reviewer will add comments on the code.**
- **PR author will reply** (for example, confirm that requested changes have been implemented or discuss why a suggested change may not be appropriate).
- **Reviewer will resolve comments** once satisfied with the changes or explanations.

Additional information can be found in [PULL_REQUEST_TEMPLATE](https://github.com/solarwinds/net-changesets/blob/main/.github/PULL_REQUEST_TEMPLATE.md).

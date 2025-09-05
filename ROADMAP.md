# Roadmap

**`net-changesets`** is currently in the **MVP stage**, meaning it includes the basic functionality to:

- Create changesets
- Bump versions and generates changelogs
- Push packages to a specified NuGet source

Unfortunately, these actions must currently be performed **manually** via the CLI tool `net-changeset`.

Several features and improvements are planned for future releases.

## 🚧 net-changesets GitHub Action

The main missing component of net-changesets is the GitHub action that will automate the process
of creating a pull request with updated package versions and changelogs based on the changesets in your repository.
For reference, see the original implementation of the [changesets action](https://github.com/changesets/action).

We can use **.NET** to develop a custom GitHub Action — see the official [documentation](https://learn.microsoft.com/en-us/dotnet/devops/create-dotnet-github-action).

The plan is to develop the GitHub Action **within this repository**, allowing us to reuse as much existing code as possible.

## Other features

While no specific features are planned at the moment, there is plenty of room for improving the existing functionality.
If you have an idea or suggestion, feel free to [open a GitHub issue](./issues) to start a discussion.

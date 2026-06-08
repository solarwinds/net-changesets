---
"SolarWinds.Changesets": Patch
---

Upgrade Spectre.Console to 0.56.0 and Spectre.Console.Cli to 0.55.0 (with matching testing packages). Adapts to the new command `ExecuteAsync(CommandContext, CancellationToken)` signature and the move of `CommandAppTester` into the new `Spectre.Console.Cli.Testing` package.

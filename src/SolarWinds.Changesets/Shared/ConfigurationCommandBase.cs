using Spectre.Console.Cli;

namespace SolarWinds.Changesets.Shared;

internal abstract class ConfigurationCommandBase : AsyncCommand
{
    private readonly IConfigurationService _configurationService;

    /// <summary>
    /// Changeset configuration.
    /// </summary>
    protected ChangesetConfig ChangesetConfig { get; private set; } = default!;

    protected ConfigurationCommandBase(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    public sealed override async Task<int> ExecuteAsync(CommandContext context)
    {
        if (!Directory.Exists(Constants.ChangesetDirectoryFullPath))
        {
            throw new InitializationException(
                $"'.changeset' directory does not exist on following path '{Constants.ChangesetDirectoryFullPath}'.");
        }

        ChangesetConfig = await _configurationService.GetConfigAsync(Constants.ChangesetConfigFileFullPath);

        return await ExecuteCommandAsync(context);
    }

    public abstract Task<int> ExecuteCommandAsync(CommandContext context);
}

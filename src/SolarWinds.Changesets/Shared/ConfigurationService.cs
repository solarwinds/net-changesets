using System.Text.Json;

namespace SolarWinds.Changesets.Shared;

/// <inheritdoc />
internal sealed class ConfigurationService : IConfigurationService
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    /// <inheritdoc />
    public async Task CreateDefaultAsync(string path)
    {
        ChangesetConfig config = new();

        string json = JsonSerializer.Serialize(config, _options);

        await File.AppendAllTextAsync(path, json);
    }

    /// <inheritdoc />
    public async Task<ChangesetConfig> GetConfigAsync(string path)
    {
        string configFileContent = await File.ReadAllTextAsync(path);

        ChangesetConfig? config = JsonSerializer.Deserialize<ChangesetConfig>(configFileContent, _options);
        return config ?? throw new InvalidOperationException("Could not deserialize changeset config.");
    }
}

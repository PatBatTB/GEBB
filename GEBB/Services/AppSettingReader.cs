using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Com.Github.PatBatTB.GEBB.Services;

public class AppSettingReader : IConfiguration
{
    private readonly IConfiguration _configurationImplementation;

    public AppSettingReader()
    {
        _configurationImplementation = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public IEnumerable<IConfigurationSection> GetChildren()
    {
        return _configurationImplementation.GetChildren();
    }

    public IChangeToken GetReloadToken()
    {
        return _configurationImplementation.GetReloadToken();
    }

    public IConfigurationSection GetSection(string key)
    {
        return _configurationImplementation.GetSection(key);
    }

    public string? this[string key]
    {
        get => _configurationImplementation[key];
        set => _configurationImplementation[key] = value;
    }
}
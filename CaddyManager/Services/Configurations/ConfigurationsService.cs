using CaddyManager.Configurations.Caddy;
using CaddyManager.Configurations.Docker;
using CaddyManager.Contracts.Configurations;
using NetCore.AutoRegisterDi;

namespace CaddyManager.Services.Configurations;

/// <inheritdoc />
[RegisterAsSingleton]
public class ConfigurationsService(IConfiguration configuration) : IConfigurationsService
{
    /// <inheritdoc />
    public T Get<T>() where T : class
    {
        var section = typeof(T).Name;
        
        // Have the configuration section name be the section name without the "Configurations" suffix
        if (section.EndsWith("Configurations"))
            section = section[..^"Configurations".Length];
        else if (section.EndsWith("Configuration"))
            section = section[..^"Configuration".Length];
        
        return configuration.GetSection(section).Get<T>() ?? Activator.CreateInstance<T>();
    }
}
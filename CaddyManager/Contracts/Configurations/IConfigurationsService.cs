using CaddyManager.Configurations.Caddy;

namespace CaddyManager.Contracts.Configurations;

/// <summary>
/// Contract for the services providing the configurations for the application
/// </summary>
public interface IConfigurationsService
{
    /// <summary>
    /// Configurations for Caddy service
    /// </summary>
    CaddyServiceConfigurations CaddyServiceConfigurations { get; }
}
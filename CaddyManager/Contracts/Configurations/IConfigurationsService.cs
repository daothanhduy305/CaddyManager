using CaddyManager.Configurations.Caddy;
using CaddyManager.Configurations.Docker;

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
    
    /// <summary>
    /// Configurations for Docker service
    /// </summary>
    DockerServiceConfiguration DockerServiceConfiguration { get; }
}
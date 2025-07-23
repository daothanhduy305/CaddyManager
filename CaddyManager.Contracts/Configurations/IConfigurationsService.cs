using CaddyManager.Contracts.Configurations.Caddy;
using CaddyManager.Contracts.Configurations.Docker;

namespace CaddyManager.Contracts.Configurations;

/// <summary>
/// Contract for the services providing the configurations for the application
/// </summary>
public interface IConfigurationsService
{
    /// <summary>
    /// Method extracting the configurations from the appsettings.json file or environment variables base on the
    /// type of the configuration class to determine the section name
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T Get<T>() where T : class;
}
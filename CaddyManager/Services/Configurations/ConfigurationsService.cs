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
    public CaddyServiceConfigurations CaddyServiceConfigurations =>
        configuration.GetSection(CaddyServiceConfigurations.Caddy).Get<CaddyServiceConfigurations>() ??
        new CaddyServiceConfigurations();

    public DockerServiceConfiguration DockerServiceConfiguration =>
        configuration.GetSection(DockerServiceConfiguration.Docker).Get<DockerServiceConfiguration>() ??
        new DockerServiceConfiguration();
}
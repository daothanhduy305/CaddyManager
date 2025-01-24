using CaddyManager.Configurations.Docker;
using CaddyManager.Contracts.Configurations;
using CaddyManager.Contracts.Docker;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace CaddyManager.Services.Docker;

/// <inheritdoc />
public class DockerService(IConfigurationsService configurationsService) : IDockerService
{
    private DockerServiceConfiguration Configuration => configurationsService.DockerServiceConfiguration;

    /// <summary>
    /// Method to get the container id of the Caddy container by the name configured
    /// </summary>
    /// <returns></returns>
    private async Task<string> GetCaddyContainerId()
    {
        var client = new DockerClientConfiguration(new Uri(Configuration.DockerHostWithEnvCheck)).CreateClient();

        if (client == null) return string.Empty;
        
        var containers = await client.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true
        });

        return containers.FirstOrDefault(container => container.Names.Contains($"/{Configuration.CaddyContainerName}"))
            ?.ID ?? string.Empty;

    }
    
    /// <inheritdoc />
    public async Task RestartCaddyContainerAsync()
    {
        var containerId = await GetCaddyContainerId();
        
        if (string.IsNullOrEmpty(containerId)) return;
        
        var client = new DockerClientConfiguration(new Uri(Configuration.DockerHostWithEnvCheck)).CreateClient();

        if (client != null)
        {
            await client.Containers.RestartContainerAsync(containerId, new ContainerRestartParameters());
        }
    }
}
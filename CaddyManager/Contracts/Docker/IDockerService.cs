namespace CaddyManager.Contracts.Docker;

/// <summary>
/// Contract for the service to interact with Docker
/// </summary>
public interface IDockerService
{
    /// <summary>
    /// Method to help restart the Caddy container
    /// </summary>
    /// <returns></returns>
    Task RestartCaddyContainerAsync();
}
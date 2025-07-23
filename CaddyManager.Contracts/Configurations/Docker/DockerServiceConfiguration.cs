namespace CaddyManager.Contracts.Configurations.Docker;

/// <summary>
/// Configuration for the Docker service
/// </summary>
public class DockerServiceConfiguration
{
    public const string Docker = "Docker";

    /// <summary>
    /// Name of the Caddy container to be controlled (i.e restart)
    /// </summary>
    public string CaddyContainerName { get; set; } = "caddy";
    
    /// <summary>
    /// Uri to the Docker host
    /// </summary>
    public string DockerHost { get; set; } = "unix:///var/run/docker.sock";
    
    /// <summary>
    /// Returns the Docker host with environment check. If the environment variable DOCKER_HOST is set, it will return
    /// that value, otherwise it will return the value of DockerHost
    /// </summary>
    public string DockerHostWithEnvCheck
    {
        get
        {
            var envValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
            return string.IsNullOrWhiteSpace(envValue) ? DockerHost : envValue;
        }
    }
}
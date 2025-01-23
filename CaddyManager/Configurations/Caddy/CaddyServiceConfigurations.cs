namespace CaddyManager.Configurations.Caddy;

/// <summary>
/// Wraps the configurations for Caddy service
/// </summary>
public class CaddyServiceConfigurations
{
    public const string Caddy = "Caddy";
    
    public string ConfigDir { get; set; } = "/config";
}
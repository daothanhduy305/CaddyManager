namespace CaddyManager.Models.Caddy;

/// <summary>
/// Wraps the information parsed from the Caddy configuration file.
/// </summary>
public class CaddyConfigurationInfo
{
    /// <summary>
    /// Hostnames that are configured in the Caddyfile.
    /// </summary>
    public List<string> Hostnames { get; set; } = [];
    
    /// <summary>
    /// The hostname of the reverse proxy server.
    /// </summary>
    public string ReverseProxyHostname { get; set; } = string.Empty;
    
    /// <summary>
    /// Ports being used with the reverse proxy hostname
    /// </summary>
    public List<int> ReverseProxyPorts { get; set; } = [];
}
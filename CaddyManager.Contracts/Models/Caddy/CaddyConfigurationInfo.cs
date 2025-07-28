namespace CaddyManager.Contracts.Models.Caddy;

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

    /// <summary>
    /// The name of the configuration file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Aggregated ports for the reverse proxy hostname from all configurations.
    /// </summary>
    public List<int> AggregatedReverseProxyPorts { get; set; } = [];

    /// <summary>
    /// Tags extracted from the configuration content using the format: # Tags: [tag1;tag2;tag3]
    /// </summary>
    public List<string> Tags { get; set; } = [];

    public override bool Equals(object? obj)
    {
        if (obj is not CaddyConfigurationInfo other)
            return false;
        return FileName == other.FileName;
    }

    public override int GetHashCode()
    {
        return FileName.GetHashCode();
    }
}
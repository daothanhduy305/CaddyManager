namespace CaddyManager.Contracts.Models.Caddy;

/// <summary>
/// Wraps the information needed to save a Caddy configuration
/// </summary>
public class CaddySaveConfigurationRequest
{
    /// <summary>
    /// Indicates if the configuration is new
    /// </summary>
    public bool IsNew { get; set; }
    
    /// <summary>
    /// Name of the Caddy configuration file
    /// </summary>
    public required string FileName { get; init; } = string.Empty;
    
    /// <summary>
    /// Content of the Caddy configuration file
    /// </summary>
    public string Content { get; set; } = string.Empty;
}
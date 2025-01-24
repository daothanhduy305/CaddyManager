namespace CaddyManager.Models.Caddy;

/// <summary>
/// Class to wrap the response of a Caddy delete operation
/// </summary>
public class CaddyDeleteOperationResponse : CaddyOperationResponse
{
    /// <summary>
    /// List of configurations that were successfully deleted
    /// </summary>
    public List<string> DeletedConfigurations { get; set; } = [];
}
namespace CaddyManager.Contracts.Models.Caddy;

/// <summary>
/// Class to wrap the response of a generic Caddy operation
/// </summary>
public class CaddyOperationResponse
{
    private string _message = string.Empty;

    /// <summary>
    /// Indicates if the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Message to describe the operation result to provide more context
    /// </summary>
    public string Message
    {
        get => _message;
        set => _message = value ?? string.Empty;
    }
}
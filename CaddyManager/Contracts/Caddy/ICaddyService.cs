namespace CaddyManager.Contracts.Caddy;

/// <summary>
/// Contracts for Caddy Service to help monitor the available Caddy configurations
/// </summary>
public interface ICaddyService
{
    /// <summary>
    /// Returns the existing Caddy configurations within the configured directory
    /// </summary>
    /// <returns></returns>
    List<string> GetExistingCaddyConfigurations();
}
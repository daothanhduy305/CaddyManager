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
    
    /// <summary>
    /// Method to get the content of a Caddy configuration file by its name
    /// The expected path to be [ConfigDir]/[configurationName].caddy
    /// </summary>
    /// <param name="configurationName"></param>
    /// <returns></returns>
    string GetCaddyConfigurationContent(string configurationName);
    
    /// <summary>
    /// Method to get the content of the global Caddy configuration file
    /// </summary>
    /// <returns></returns>
    string GetCaddyGlobalConfigurationContent();
}
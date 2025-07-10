namespace CaddyManager.Contracts.Caddy;

/// <summary>
/// Contract for a service that parses Caddy configuration files.
/// </summary>
public interface ICaddyConfigurationParsingService
{
    /// <summary>
    /// Extracts outermost hostname declarations from a Caddyfile content.
    /// i.e.
    /// ```
    /// caddy.domain.name {
    ///     route {
    ///         reverse_proxy localhost:8080
    ///         encode zstd gzip
    ///     }
    /// }
    /// ```
    /// will return `["caddy.domain.name"]`.
    /// </summary>
    /// <param name="caddyfileContent"></param>
    /// <returns></returns>
    List<string> GetHostnamesFromCaddyfileContent(string caddyfileContent);
    
    /// <summary>
    /// Extracts the reverse proxy target from a Caddyfile content.
    /// </summary>
    /// <param name="caddyfileContent"></param>
    /// <returns></returns>
    string GetReverseProxyTargetFromCaddyfileContent(string caddyfileContent);
    
    /// <summary>
    /// Extracts the ports being used with the reverse proxy host
    /// </summary>
    /// <param name="caddyfileContent"></param>
    /// <returns></returns>
    List<int> GetReverseProxyPortsFromCaddyfileContent(string caddyfileContent);
}
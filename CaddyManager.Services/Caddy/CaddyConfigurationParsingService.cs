using System.Text.RegularExpressions;
using CaddyManager.Contracts.Caddy;

namespace CaddyManager.Services.Caddy;

/// <inheritdoc />
public partial class CaddyConfigurationParsingService: ICaddyConfigurationParsingService
{
    /// <summary>
    /// Regex to help parse hostnames from a Caddyfile.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?m)^\s*([^\{\r\n]+?)\s*\{", RegexOptions.Multiline)]
    private static partial Regex HostnamesRegex();
    
    /// <summary>
    /// Regex to help parse hostnames being used in reverse proxy directives.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?m)reverse_proxy\s+([^\s\{\}]+)(?:\s*\{)?", RegexOptions.Multiline)]
    private static partial Regex ReverseProxyRegex();

    /// <inheritdoc />
    public List<string> GetHostnamesFromCaddyfileContent(string caddyfileContent)
    {
        var hostnamesRegex = HostnamesRegex();
        var matches = hostnamesRegex.Matches(caddyfileContent);
        var hostnames = new List<string>();
        foreach (Match match in matches)
        {
            // Split the matched string by commas and trim whitespace
            var splitHostnames = match.Groups[1].Value.Split(',')
                .Select(h => h.Trim())
                .Where(h => !string.IsNullOrWhiteSpace(h))
                .ToList();

            hostnames.AddRange(splitHostnames);
        }
        // Remove duplicates and return the list
        return hostnames.Distinct().ToList();
    }

    /// <inheritdoc />
    public string GetReverseProxyTargetFromCaddyfileContent(string caddyfileContent)
    {
        var reverseProxyRegex = ReverseProxyRegex();
        var match = reverseProxyRegex.Match(caddyfileContent);
        if (!match.Success) return string.Empty;

        // Use the captured group which contains the target (e.g., pikachu:3011)
        var targetPart = match.Groups[1].Value.Trim();
        if (string.IsNullOrEmpty(targetPart)) return string.Empty;

        var targetComponents = targetPart.Split(':');
        if (targetComponents.Length <= 1) return targetPart;

        // Handle cases like http://backend:9000, 192.168.1.100:3000
        return targetComponents[0];
    }

    /// <inheritdoc />
    public List<int> GetReverseProxyPortsFromCaddyfileContent(string caddyfileContent)
    {
        var reverseProxyRegex = ReverseProxyRegex();
        var matches = reverseProxyRegex.Matches(caddyfileContent);
        var results = new List<int>();

        foreach (Match match in matches)
        {
            var parts = match.Value.TrimEnd('}').Trim().Split(' ');
            var targetPart = parts.LastOrDefault(string.Empty);
            if (string.IsNullOrEmpty(targetPart)) continue;

            var targetComponents = targetPart.Split(':');
            if (targetComponents.Length > 1 && int.TryParse(targetComponents.Last(), out int port))
            {
                results.Add(port);
            }
        }

        return results.Distinct().ToList();
    }
}
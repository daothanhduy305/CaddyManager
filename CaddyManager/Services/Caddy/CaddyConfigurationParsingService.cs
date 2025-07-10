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
    [GeneratedRegex(@"(?m)^[\w.-]+(?:\s*,\s*[\w.-]+)*(?=\s*\{)", RegexOptions.Multiline)]
    private static partial Regex HostnamesRegex();
    
    /// <summary>
    /// Regex to help parse hostnames being used in reverse proxy directives.
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?m)reverse_proxy .*", RegexOptions.Multiline)]
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
            var splitHostnames = match.Value.Split(',')
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
        return match.Value.TrimEnd('{').Trim().Split(' ').LastOrDefault(string.Empty).Split(':')
            .FirstOrDefault(string.Empty);
    }

    /// <inheritdoc />
    public List<int> GetReverseProxyPortsFromCaddyfileContent(string caddyfileContent)
    {
        var reverseProxyRegex = ReverseProxyRegex();
        var matches = reverseProxyRegex.Matches(caddyfileContent);
        var results = new List<int>();

        foreach (Match match in matches)
        {
            results.Add(int.Parse(match.Value.TrimEnd('{').Trim().Split(' ').LastOrDefault(string.Empty).Split(':')
                .LastOrDefault(string.Empty)));
        }

        return results.Distinct().ToList();
    }
}
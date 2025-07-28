using System.Text.RegularExpressions;
using CaddyManager.Contracts.Caddy;

namespace CaddyManager.Services.Caddy;

/// <inheritdoc />
public partial class CaddyConfigurationParsingService: ICaddyConfigurationParsingService
{
    /// <summary>
    /// Regex to help parse hostnames from a Caddyfile.
    /// This regex only matches hostname declarations at the beginning of lines (column 1 after optional whitespace)
    /// and excludes nested directives like "reverse_proxy target {".
    /// </summary>
    /// <returns></returns>
    [GeneratedRegex(@"(?m)^([^\s\{\r\n][^\{\r\n]*?)\s*\{", RegexOptions.Multiline)]
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
        return [.. hostnames.Distinct()];
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
            // Use the captured group which contains the target (e.g., pikachu:3011)
            var targetPart = match.Groups[1].Value.Trim();
            if (string.IsNullOrEmpty(targetPart)) continue;

            var targetComponents = targetPart.Split(':');
            if (targetComponents.Length > 1 && int.TryParse(targetComponents.Last(), out int port))
            {
                results.Add(port);
            }
        }

        return [.. results.Distinct()];
    }

    /// <inheritdoc />
    public List<string> GetTagsFromCaddyfileContent(string caddyfileContent)
    {
        // Split the content into lines and look for the tags line
        var lines = caddyfileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("#"))
            {
                // Remove the # and any leading whitespace, then check if it starts with "tags:"
                var afterHash = trimmedLine.Substring(1).TrimStart();
                if (afterHash.StartsWith("tags:", StringComparison.OrdinalIgnoreCase))
                {
                    // Extract the part after "tags:"
                    var tagsString = afterHash.Substring(5).Trim(); // 5 = length of "tags:"
                    if (string.IsNullOrWhiteSpace(tagsString))
                        return [];

                    // Split by semicolon and clean up each tag
                    return [.. tagsString.Split(';')
                        .Select(tag => tag.Trim())
                        .Where(tag => !string.IsNullOrWhiteSpace(tag))
                        .Distinct()];
                }
            }
        }

        // No tags line found
        return [];
    }
}
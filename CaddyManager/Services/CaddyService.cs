using CaddyManager.Configurations.Caddy;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Configurations;

namespace CaddyManager.Services;

/// <inheritdoc />
public class CaddyService(IConfigurationsService configurationsService) : ICaddyService
{
    /// <summary>
    /// File name of the global configuration Caddyfile
    /// </summary>
    public const string CaddyGlobalConfigName = "Caddyfile";
    
    private CaddyServiceConfigurations Configurations => configurationsService.CaddyServiceConfigurations;
    
    /// <inheritdoc />
    public List<string> GetExistingCaddyConfigurations()
    {
        return Directory.GetFiles(Configurations.ConfigDir)
            .Where(filePath => Path.GetFileName(filePath) != CaddyGlobalConfigName)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList()!;
    }

    /// <inheritdoc />
    public string GetCaddyConfigurationContent(string configurationName)
    {
        var path = configurationName == CaddyGlobalConfigName 
            ? Path.Combine(Configurations.ConfigDir, CaddyGlobalConfigName) 
            : Path.Combine(Configurations.ConfigDir, $"{configurationName}.caddy");
        
        if (File.Exists(path))
        {
            return File.ReadAllText(path);
        }
        
        return string.Empty;
    }

    /// <inheritdoc />
    public string GetCaddyGlobalConfigurationContent() => GetCaddyConfigurationContent(CaddyGlobalConfigName);
}
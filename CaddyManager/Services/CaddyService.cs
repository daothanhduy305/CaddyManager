using CaddyManager.Configurations.Caddy;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Configurations;

namespace CaddyManager.Services;

/// <inheritdoc />
public class CaddyService(IConfigurationsService configurationsService) : ICaddyService
{
    private CaddyServiceConfigurations _configurations => configurationsService.CaddyServiceConfigurations;
    
    /// <inheritdoc />
    public List<string> GetExistingCaddyConfigurations()
    {
        return Directory.GetFiles(_configurations.ConfigDir).ToList();
    }
}
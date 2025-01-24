using CaddyManager.Configurations.Caddy;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Configurations;
using CaddyManager.Models.Caddy;

namespace CaddyManager.Services;

/// <inheritdoc />
public class CaddyService(IConfigurationsService configurationsService) : ICaddyService
{
    /// <summary>
    /// File name of the global configuration Caddyfile
    /// </summary>
    private const string CaddyGlobalConfigName = "Caddyfile";

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

    /// <inheritdoc />
    public CaddyOperationResponse SaveCaddyConfiguration(CaddySaveConfigurationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FileName))
        {
            return new CaddyOperationResponse
            {
                Success = false,
                Message = "The configuration file name is required"
            };
        }

        var filePath = Path.Combine(Configurations.ConfigDir,
            request.FileName == CaddyGlobalConfigName ? CaddyGlobalConfigName : $"{request.FileName}.caddy");
        // if in the new mode, we would have to check if the file already exists
        if (request.IsNew && File.Exists(filePath))
        {
            return new CaddyOperationResponse
            {
                Success = false,
                Message = "The configuration file already exists"
            };
        }

        try
        {
            File.WriteAllText(filePath, request.Content);
            return new CaddyOperationResponse
            {
                Success = true,
                Message = "Configuration file saved successfully"
            };
        }
        catch (Exception e)
        {
            return new CaddyOperationResponse
            {
                Success = false,
                Message = e.Message
            };
        }
    }

    /// <inheritdoc />
    public CaddyOperationResponse SaveCaddyGlobalConfiguration(string content) => SaveCaddyConfiguration(
        new CaddySaveConfigurationRequest
        {
            FileName = CaddyGlobalConfigName,
            Content = content
        });
}
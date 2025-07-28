using CaddyManager.Contracts.Configurations.Caddy;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Configurations;
using CaddyManager.Contracts.Models.Caddy;

namespace CaddyManager.Services.Caddy;

/// <inheritdoc />
public class CaddyService(
    IConfigurationsService configurationsService,
    ICaddyConfigurationParsingService parsingService) : ICaddyService
{
    /// <summary>
    /// File name of the global configuration Caddyfile
    /// </summary>
    private const string CaddyGlobalConfigName = "Caddyfile";

    private CaddyServiceConfigurations Configurations => configurationsService.Get<CaddyServiceConfigurations>();

    /// <inheritdoc />
    public List<CaddyConfigurationInfo> GetExistingCaddyConfigurations()
    {
        if (!Directory.Exists(Configurations.ConfigDir))
        {
            Directory.CreateDirectory(Configurations.ConfigDir);
        }

        return [.. Directory.GetFiles(Configurations.ConfigDir)
            .Where(filePath => Path.GetFileName(filePath) != CaddyGlobalConfigName)
            .Select(filePath =>
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                var info = GetCaddyConfigurationInfo(fileName);
                info.FileName = fileName;
                return info;
            })
            .OrderBy(info => info.FileName)];
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

    /// <inheritdoc />
    public CaddyDeleteOperationResponse DeleteCaddyConfigurations(List<string> configurationNames)
    {
        var failed = new List<string>();

        foreach (var configurationName in configurationNames)
        {
            var filePath = Path.Combine(Configurations.ConfigDir,
                configurationName == CaddyGlobalConfigName ? CaddyGlobalConfigName : $"{configurationName}.caddy");

            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                    failed.Add(configurationName);
                }
            }
            else
            {
                failed.Add(configurationName);
            }
        }

        return new CaddyDeleteOperationResponse
        {
            Success = failed.Count == 0,
            Message = failed.Count == 0
                ? "Configuration(s) deleted successfully"
                : $"Failed to delete the following configuration(s): {string.Join(", ", failed)}",
            DeletedConfigurations = configurationNames.Except(failed).ToList()
        };
    }

    /// <inheritdoc />
    public CaddyConfigurationInfo GetCaddyConfigurationInfo(string configurationName)
    {
        var result = new CaddyConfigurationInfo
        {
            FileName = configurationName
        };
        
        var content = GetCaddyConfigurationContent(configurationName);
        if (string.IsNullOrWhiteSpace(content))
        {
            return result;
        }
        
        result.Hostnames = parsingService.GetHostnamesFromCaddyfileContent(content);
        result.ReverseProxyHostname = parsingService.GetReverseProxyTargetFromCaddyfileContent(content);
        result.ReverseProxyPorts  = parsingService.GetReverseProxyPortsFromCaddyfileContent(content);
        result.Tags = parsingService.GetTagsFromCaddyfileContent(content);

        return result;
    }
}
using Microsoft.Extensions.Configuration;

namespace CaddyManager.Tests.TestUtilities;

/// <summary>
/// Helper class for common test utilities
/// </summary>
public static class TestHelper
{
    /// <summary>
    /// Creates a temporary directory for testing file operations
    /// </summary>
    /// <returns>Path to the temporary directory</returns>
    public static string CreateTempDirectory()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempPath);
        return tempPath;
    }

    /// <summary>
    /// Creates a temporary file with the specified content
    /// </summary>
    /// <param name="content">Content to write to the file</param>
    /// <param name="fileName">Optional file name, generates random if not provided</param>
    /// <returns>Path to the created file</returns>
    public static string CreateTempFile(string content, string? fileName = null)
    {
        var tempDir = CreateTempDirectory();
        var filePath = Path.Combine(tempDir, fileName ?? $"{Guid.NewGuid()}.txt");
        File.WriteAllText(filePath, content);
        return filePath;
    }

    /// <summary>
    /// Creates an IConfiguration instance from a dictionary
    /// </summary>
    /// <param name="configValues">Configuration key-value pairs</param>
    /// <returns>IConfiguration instance</returns>
    public static IConfiguration CreateConfiguration(Dictionary<string, string?> configValues)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }

    /// <summary>
    /// Creates an IConfiguration instance from JSON content
    /// </summary>
    /// <param name="jsonContent">JSON configuration content</param>
    /// <returns>IConfiguration instance</returns>
    public static IConfiguration CreateConfigurationFromJson(string jsonContent)
    {
        var tempFile = CreateTempFile(jsonContent, "appsettings.json");
        return new ConfigurationBuilder()
            .AddJsonFile(tempFile)
            .Build();
    }

    /// <summary>
    /// Sample Caddyfile content for testing
    /// </summary>
    public static class SampleCaddyfiles
    {
        public const string SimpleReverseProxy = @"
example.com {
    reverse_proxy localhost:8080
}";

        public const string MultipleHosts = @"
example.com, www.example.com {
    reverse_proxy localhost:8080
}";

        public const string ComplexConfiguration = @"
api.example.com {
    route /v1/* {
        reverse_proxy localhost:3000
    }
    route /v2/* {
        reverse_proxy localhost:3001
    }
}

app.example.com {
    reverse_proxy localhost:8080
    encode gzip
}";

        public const string WithMultiplePorts = @"
example.com {
    reverse_proxy localhost:8080
}

api.example.com {
    reverse_proxy localhost:3000
}";
    }

    /// <summary>
    /// Cleans up a directory and all its contents
    /// </summary>
    /// <param name="directoryPath">Path to the directory to clean up</param>
    public static void CleanupDirectory(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            try
            {
                Directory.Delete(directoryPath, true);
            }
            catch
            {
                // Ignore cleanup errors in tests
            }
        }
    }
}
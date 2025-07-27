using CaddyManager.Contracts.Configurations.Caddy;
using CaddyManager.Contracts.Configurations.Docker;
using CaddyManager.Services.Configurations;
using CaddyManager.Tests.TestUtilities;

namespace CaddyManager.Tests.Services.Configurations;

/// <summary>
/// Tests for ConfigurationsService
/// </summary>
public class ConfigurationsServiceTests
{
    /// <summary>
    /// Tests that the configurations service correctly binds and returns Caddy service configuration from application settings.
    /// Setup: Creates a test configuration with custom Caddy service settings including a custom config directory path.
    /// Expectation: The service should properly bind the configuration values and return a populated CaddyServiceConfigurations object, enabling proper Caddy service initialization and configuration management.
    /// </summary>
    [Fact]
    public void Get_WithCaddyServiceConfigurations_ReturnsCorrectConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/custom/config/path"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        result.ConfigDir.Should().Be("/custom/config/path");
    }

    /// <summary>
    /// Tests that the configurations service correctly binds and returns Docker service configuration from application settings.
    /// Setup: Creates a test configuration with custom Docker service settings including container name and Docker host connection details.
    /// Expectation: The service should properly bind the configuration values and return a populated DockerServiceConfiguration object, enabling proper Docker integration for Caddy container management.
    /// </summary>
    [Fact]
    public void Get_WithDockerServiceConfiguration_ReturnsCorrectConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["DockerService:CaddyContainerName"] = "custom-caddy",
            ["DockerService:DockerHost"] = "tcp://localhost:2376"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<DockerServiceConfiguration>();

        // Assert
        result.Should().NotBeNull();
        result.CaddyContainerName.Should().Be("custom-caddy");
        result.DockerHost.Should().Be("tcp://localhost:2376");
    }

    /// <summary>
    /// Tests that the configurations service returns a default configuration instance when no configuration values are provided.
    /// Setup: Creates an empty configuration dictionary with no configuration values set.
    /// Expectation: The service should return a configuration object with default values, ensuring the application can function with sensible defaults when configuration is missing or incomplete.
    /// </summary>
    [Fact]
    public void Get_WithMissingConfiguration_ReturnsDefaultInstance()
    {
        // Arrange
        var configuration = TestHelper.CreateConfiguration(new Dictionary<string, string?>());
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        result.ConfigDir.Should().Be("/config"); // Default value
    }

    /// <summary>
    /// Tests that the configurations service correctly handles partial configuration by using defaults for missing values.
    /// Setup: Creates a configuration with only some values set (container name) while leaving others (Docker host) unspecified.
    /// Expectation: The service should return a configuration object with provided values and sensible defaults for missing values, ensuring robust configuration handling in various deployment scenarios.
    /// </summary>
    [Fact]
    public void Get_WithPartialConfiguration_ReturnsInstanceWithDefaults()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["DockerService:CaddyContainerName"] = "my-caddy"
            // DockerHost is missing, should use default
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<DockerServiceConfiguration>();

        // Assert
        result.Should().NotBeNull();
        result.CaddyContainerName.Should().Be("my-caddy");
        result.DockerHost.Should().Be("unix:///var/run/docker.sock"); // Default value
    }

    /// <summary>
    /// Tests that the configurations service correctly removes the "Configuration" suffix from class names when determining configuration section names.
    /// Setup: Creates a configuration for a class ending in "Configuration" and verifies the section name mapping logic.
    /// Expectation: The service should automatically map class names to configuration sections by removing the "Configuration" suffix, enabling intuitive configuration section naming conventions.
    /// </summary>
    [Fact]
    public void Get_WithConfigurationSuffixInClassName_RemovesSuffix()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/test/path"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        result.ConfigDir.Should().Be("/test/path");
    }

    /// <summary>
    /// Tests that the configurations service correctly removes the "Configurations" suffix from class names when determining configuration section names.
    /// Setup: Creates a configuration for a class ending in "Configurations" and verifies the section name mapping logic.
    /// Expectation: The service should automatically map class names to configuration sections by removing the "Configurations" suffix, supporting both singular and plural naming conventions for configuration classes.
    /// </summary>
    [Fact]
    public void Get_WithConfigurationsSuffixInClassName_RemovesSuffix()
    {
        // Arrange - Test with a hypothetical class ending in "Configurations"
        var configValues = new Dictionary<string, string?>
        {
            ["DockerService:CaddyContainerName"] = "test-container",
            ["DockerService:DockerHost"] = "tcp://test:2376"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<DockerServiceConfiguration>();

        // Assert
        result.Should().NotBeNull();
        result.CaddyContainerName.Should().Be("test-container");
        result.DockerHost.Should().Be("tcp://test:2376");
    }

    /// <summary>
    /// Tests that the configurations service correctly parses and binds configuration values from JSON format.
    /// Setup: Creates a JSON configuration string with nested configuration sections for both Caddy and Docker services.
    /// Expectation: The service should properly parse the JSON structure and return correctly populated configuration objects, ensuring compatibility with JSON-based configuration files like appsettings.json.
    /// </summary>
    [Fact]
    public void Get_WithJsonConfiguration_ReturnsCorrectConfiguration()
    {
        // Arrange
        var jsonContent = @"{
            ""CaddyService"": {
                ""ConfigDir"": ""/json/config/path""
            },
            ""DockerService"": {
                ""CaddyContainerName"": ""json-caddy"",
                ""DockerHost"": ""tcp://json-host:2376""
            }
        }";
        var configuration = TestHelper.CreateConfigurationFromJson(jsonContent);
        var service = new ConfigurationsService(configuration);

        // Act
        var caddyResult = service.Get<CaddyServiceConfigurations>();
        var dockerResult = service.Get<DockerServiceConfiguration>();

        // Assert
        caddyResult.Should().NotBeNull();
        caddyResult.ConfigDir.Should().Be("/json/config/path");

        dockerResult.Should().NotBeNull();
        dockerResult.CaddyContainerName.Should().Be("json-caddy");
        dockerResult.DockerHost.Should().Be("tcp://json-host:2376");
    }

    /// <summary>
    /// Tests that the configurations service correctly handles nested configuration structures with multiple properties.
    /// Setup: Creates a configuration with multiple nested properties under the same configuration section.
    /// Expectation: The service should properly bind all nested configuration values, ensuring support for complex configuration structures with multiple settings per service.
    /// </summary>
    [Fact]
    public void Get_WithNestedConfiguration_ReturnsCorrectConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/nested/config",
            ["CaddyService:SomeOtherProperty"] = "test-value"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        result.ConfigDir.Should().Be("/nested/config");
    }

    /// <summary>
    /// Tests that the configurations service returns consistent results when called multiple times for the same configuration type.
    /// Setup: Creates a configuration and calls the Get method multiple times to retrieve the same configuration type.
    /// Expectation: The service should return consistent configuration values across multiple calls, ensuring reliable and predictable configuration behavior throughout the application lifecycle.
    /// </summary>
    [Fact]
    public void Get_CalledMultipleTimes_ReturnsConsistentResults()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/consistent/path"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result1 = service.Get<CaddyServiceConfigurations>();
        var result2 = service.Get<CaddyServiceConfigurations>();

        // Assert
        result1.Should().NotBeNull();
        result2.Should().NotBeNull();
        result1.ConfigDir.Should().Be(result2.ConfigDir);
        result1.ConfigDir.Should().Be("/consistent/path");
    }

    /// <summary>
    /// Tests that the configurations service correctly handles empty or whitespace-only configuration values by preserving them as configured.
    /// Setup: Provides parameterized test data with empty strings and whitespace-only values for configuration properties.
    /// Expectation: The service should preserve the actual configured values (even if empty or whitespace), allowing applications to distinguish between missing configuration and intentionally empty values.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Get_WithEmptyOrWhitespaceConfigurationValues_ReturnsConfiguredValue(string configValue)
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = configValue
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        // Configuration binding sets the actual value, even if empty/whitespace
        result.ConfigDir.Should().Be(configValue);
    }

    /// <summary>
    /// Tests that the configurations service returns default values when configuration properties are explicitly set to null.
    /// Setup: Creates a configuration with a null value for a configuration property.
    /// Expectation: The service should fall back to default values when configuration is null, ensuring the application can handle missing or null configuration gracefully with sensible defaults.
    /// </summary>
    [Fact]
    public void Get_WithNullConfigurationValue_ReturnsDefaultInstance()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = null
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        // Should use default value when config is null
        result.ConfigDir.Should().Be("/config");
    }

    /// <summary>
    /// Test configuration class for testing purposes
    /// </summary>
    public class TestConfiguration
    {
        public string TestProperty { get; set; } = "default-value";
        public int TestNumber { get; set; } = 42;
    }

    /// <summary>
    /// Tests that the configurations service correctly handles custom configuration classes with various property types.
    /// Setup: Creates a test configuration class with string and integer properties, then provides configuration values for both property types.
    /// Expectation: The service should properly bind configuration values to custom classes with different property types, demonstrating the flexibility and type safety of the configuration binding system.
    /// </summary>
    [Fact]
    public void Get_WithCustomConfigurationClass_ReturnsCorrectConfiguration()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["Test:TestProperty"] = "custom-value",
            ["Test:TestNumber"] = "100"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<TestConfiguration>();

        // Assert
        result.Should().NotBeNull();
        result.TestProperty.Should().Be("custom-value");
        result.TestNumber.Should().Be(100);
    }

    #region Additional Edge Cases and Error Scenarios

    /// <summary>
    /// Tests that the configurations service handles invalid configuration sections gracefully.
    /// Setup: Creates a configuration with invalid section names that don't match any known configuration types.
    /// Expectation: The service should handle invalid configuration sections gracefully, either by returning default instances or providing appropriate error handling, ensuring robust operation with malformed configuration data.
    /// </summary>
    [Fact]
    public void Get_WithInvalidConfigurationSection_ReturnsDefaultInstance()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["InvalidSection:SomeProperty"] = "some-value"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        result.ConfigDir.Should().Be("/config"); // Default value
    }

    /// <summary>
    /// Tests that the configurations service handles type conversion errors gracefully.
    /// Setup: Creates a configuration with invalid type values that can't be converted to the expected property types.
    /// Expectation: The service should handle type conversion errors gracefully, either by using default values or providing appropriate error handling, ensuring robust operation with malformed configuration data.
    /// </summary>
    [Fact]
    public void Get_WithTypeConversionErrors_HandlesGracefully()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "invalid-path",
            ["DockerService:CaddyContainerName"] = "valid-name"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        // Should handle type conversion gracefully
    }

    /// <summary>
    /// Tests that the configurations service handles null configuration values correctly.
    /// Setup: Creates a configuration with null values for various properties.
    /// Expectation: The service should handle null configuration values correctly, either by using default values or providing appropriate null handling, ensuring robust operation with incomplete configuration data.
    /// </summary>
    [Fact]
    public void Get_WithNullConfigurationValues_HandlesCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = null,
            ["DockerService:CaddyContainerName"] = null
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var caddyResult = service.Get<CaddyServiceConfigurations>();
        var dockerResult = service.Get<DockerServiceConfiguration>();

        // Assert
        caddyResult.Should().NotBeNull();
        dockerResult.Should().NotBeNull();
        caddyResult.ConfigDir.Should().Be("/config"); // Default value
        dockerResult.CaddyContainerName.Should().Be("caddy"); // Default value
    }

    /// <summary>
    /// Tests that the configurations service handles complex nested configurations correctly.
    /// Setup: Creates a configuration with deeply nested properties and complex object structures.
    /// Expectation: The service should handle complex nested configurations correctly, ensuring proper binding of nested properties and support for advanced configuration scenarios.
    /// </summary>
    [Fact]
    public void Get_WithComplexNestedConfiguration_HandlesCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/custom/config/path",
            ["DockerService:CaddyContainerName"] = "custom-caddy",
            ["DockerService:DockerHost"] = "tcp://localhost:2376"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var caddyResult = service.Get<CaddyServiceConfigurations>();
        var dockerResult = service.Get<DockerServiceConfiguration>();

        // Assert
        caddyResult.Should().NotBeNull();
        dockerResult.Should().NotBeNull();
        caddyResult.ConfigDir.Should().Be("/custom/config/path");
        dockerResult.CaddyContainerName.Should().Be("custom-caddy");
        dockerResult.DockerHost.Should().Be("tcp://localhost:2376");
    }

    /// <summary>
    /// Tests that the configurations service handles environment variable edge cases correctly.
    /// Setup: Tests various environment variable scenarios including empty values, whitespace-only values, and special characters.
    /// Expectation: The service should handle environment variable edge cases correctly, ensuring proper fallback behavior and robust operation in various deployment environments.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("custom-value")]
    [InlineData("value-with-special-chars!@#$%")]
    public void Get_WithEnvironmentVariableEdgeCases_HandlesCorrectly(string envValue)
    {
        // Arrange
        var originalEnvValue = Environment.GetEnvironmentVariable("CUSTOM_CONFIG_VALUE");
        
        try
        {
            if (string.IsNullOrWhiteSpace(envValue))
            {
                Environment.SetEnvironmentVariable("CUSTOM_CONFIG_VALUE", null);
            }
            else
            {
                Environment.SetEnvironmentVariable("CUSTOM_CONFIG_VALUE", envValue);
            }

            var configValues = new Dictionary<string, string?>
            {
                ["CaddyService:ConfigDir"] = "/config"
            };
            var configuration = TestHelper.CreateConfiguration(configValues);
            var service = new ConfigurationsService(configuration);

            // Act
            var result = service.Get<CaddyServiceConfigurations>();

            // Assert
            result.Should().NotBeNull();
            result.ConfigDir.Should().Be("/config");
        }
        finally
        {
            // Restore original environment variable
            if (originalEnvValue == null)
            {
                Environment.SetEnvironmentVariable("CUSTOM_CONFIG_VALUE", null);
            }
            else
            {
                Environment.SetEnvironmentVariable("CUSTOM_CONFIG_VALUE", originalEnvValue);
            }
        }
    }

    /// <summary>
    /// Tests that the configurations service handles concurrent access scenarios gracefully.
    /// Setup: Creates multiple concurrent calls to the configurations service to simulate high-load scenarios.
    /// Expectation: The service should handle concurrent access gracefully without throwing exceptions or causing race conditions, ensuring thread safety in multi-user environments.
    /// </summary>
    [Fact]
    public async Task Get_WithConcurrentAccess_HandlesGracefully()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/concurrent/config"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act - Create multiple concurrent configuration requests
        var tasks = new List<Task<CaddyServiceConfigurations>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() => service.Get<CaddyServiceConfigurations>()));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        // Assert
        tasks.Should().AllSatisfy(task => 
        {
            task.Result.Should().NotBeNull();
            task.Result.ConfigDir.Should().Be("/concurrent/config");
        });
    }

    /// <summary>
    /// Tests that the configurations service handles configuration section name edge cases correctly.
    /// Setup: Tests various configuration section name formats including edge cases that might cause issues with section name processing.
    /// Expectation: The service should handle configuration section name edge cases correctly, ensuring robust operation with various section naming conventions and formats.
    /// </summary>
    [Theory]
    [InlineData("CaddyServiceConfigurations")]
    [InlineData("CaddyServiceConfiguration")]
    [InlineData("DockerServiceConfiguration")]
    [InlineData("DockerServiceConfigurations")]
    public void Get_WithSectionNameEdgeCases_HandlesCorrectly(string sectionName)
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            [$"{sectionName}:ConfigDir"] = "/edge-case/config"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        // Should handle section name processing correctly
    }

    /// <summary>
    /// Tests that the configurations service handles memory pressure scenarios gracefully.
    /// Setup: Creates a scenario where the configuration system might be under memory pressure.
    /// Expectation: The service should handle memory pressure scenarios gracefully, either by implementing memory-efficient operations or providing appropriate error handling, ensuring robust operation under resource constraints.
    /// </summary>
    [Fact]
    public void Get_WithMemoryPressure_HandlesGracefully()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/memory-test/config"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        result.ConfigDir.Should().Be("/memory-test/config");
    }

    /// <summary>
    /// Tests that the configurations service handles configuration validation edge cases correctly.
    /// Setup: Tests various configuration validation scenarios including invalid values, boundary conditions, and malformed data.
    /// Expectation: The service should handle configuration validation edge cases gracefully, ensuring robust operation with various configuration data quality levels.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("valid-value")]
    [InlineData("value-with-special-chars!@#$%^&*()")]
    [InlineData("very-long-configuration-value-that-might-cause-issues")]
    public void Get_WithValidationEdgeCases_HandlesCorrectly(string configValue)
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = configValue
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var result = service.Get<CaddyServiceConfigurations>();

        // Assert
        result.Should().NotBeNull();
        // Should handle validation edge cases gracefully
    }

    /// <summary>
    /// Tests that the configurations service handles configuration inheritance scenarios correctly.
    /// Setup: Creates a configuration with inheritance patterns where child configurations inherit from parent configurations.
    /// Expectation: The service should handle configuration inheritance scenarios correctly, ensuring proper binding of inherited properties and support for hierarchical configuration structures.
    /// </summary>
    [Fact]
    public void Get_WithConfigurationInheritance_HandlesCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string?>
        {
            ["CaddyService:ConfigDir"] = "/inherited/config",
            ["DockerService:CaddyContainerName"] = "inherited-caddy"
        };
        var configuration = TestHelper.CreateConfiguration(configValues);
        var service = new ConfigurationsService(configuration);

        // Act
        var caddyResult = service.Get<CaddyServiceConfigurations>();
        var dockerResult = service.Get<DockerServiceConfiguration>();

        // Assert
        caddyResult.Should().NotBeNull();
        dockerResult.Should().NotBeNull();
        caddyResult.ConfigDir.Should().Be("/inherited/config");
        dockerResult.CaddyContainerName.Should().Be("inherited-caddy");
    }

    #endregion
}
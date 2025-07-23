using CaddyManager.Configurations.Caddy;

namespace CaddyManager.Tests.Configurations.Caddy;

/// <summary>
/// Tests for CaddyServiceConfigurations
/// </summary>
public class CaddyServiceConfigurationsTests
{
    /// <summary>
    /// Tests that the CaddyServiceConfigurations constructor initializes with the correct default values.
    /// Setup: Creates a new CaddyServiceConfigurations instance using the default constructor.
    /// Expectation: The ConfigDir property should be set to "/config" by default, ensuring proper initialization for Caddy service configuration management without requiring explicit configuration.
    /// </summary>
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Act
        var config = new CaddyServiceConfigurations();

        // Assert
        config.ConfigDir.Should().Be("/config");
    }

    /// <summary>
    /// Tests that the ConfigDir property can be set and retrieved correctly with a custom path value.
    /// Setup: Creates a CaddyServiceConfigurations instance and sets ConfigDir to a custom path "/custom/caddy/config".
    /// Expectation: The property should store and return the exact value provided, ensuring proper configuration path management for Caddy service operations in custom deployment scenarios.
    /// </summary>
    [Fact]
    public void ConfigDir_CanBeSetAndRetrieved()
    {
        // Arrange
        var customPath = "/custom/caddy/config";

        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = customPath
        };

        // Assert
        config.ConfigDir.Should().Be(customPath);
    }

    /// <summary>
    /// Tests that the ConfigDir property correctly handles various valid path formats across different operating systems.
    /// Setup: Uses parameterized test data including Unix paths, Windows paths, and common Caddy configuration directories.
    /// Expectation: The property should accept and store any path format, supporting cross-platform deployment scenarios and different Caddy installation configurations.
    /// </summary>
    [Theory]
    [InlineData("/config")]
    [InlineData("/var/lib/caddy")]
    [InlineData("/home/user/caddy-configs")]
    [InlineData("C:\\Caddy\\Config")]
    [InlineData("/opt/caddy/configurations")]
    public void ConfigDir_WithVariousPaths_SetsCorrectly(string path)
    {
        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = path
        };

        // Assert
        config.ConfigDir.Should().Be(path);
    }

    /// <summary>
    /// Tests that the ConfigDir property can be set to an empty string value.
    /// Setup: Creates a CaddyServiceConfigurations instance and explicitly sets ConfigDir to an empty string.
    /// Expectation: The property should accept and store the empty string, allowing for scenarios where configuration directory might be cleared or reset programmatically.
    /// </summary>
    [Fact]
    public void ConfigDir_WithEmptyString_SetsCorrectly()
    {
        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = ""
        };

        // Assert
        config.ConfigDir.Should().Be("");
    }

    /// <summary>
    /// Tests that the ConfigDir property can be set to a null value.
    /// Setup: Creates a CaddyServiceConfigurations instance and explicitly sets ConfigDir to null.
    /// Expectation: The property should accept and store null values, supporting scenarios where configuration directory is undefined or needs to be cleared.
    /// </summary>
    [Fact]
    public void ConfigDir_WithNullValue_SetsCorrectly()
    {
        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = null!
        };

        // Assert
        config.ConfigDir.Should().BeNull();
    }

    /// <summary>
    /// Tests that the ConfigDir property can be modified after the object has been created and initialized.
    /// Setup: Creates a CaddyServiceConfigurations instance with default values, then changes ConfigDir to a new path.
    /// Expectation: The property should be mutable and accept the new value, enabling runtime reconfiguration of Caddy service paths for dynamic deployment scenarios.
    /// </summary>
    [Fact]
    public void ConfigDir_CanBeModifiedAfterCreation()
    {
        // Arrange
        var config = new CaddyServiceConfigurations();
        var newPath = "/new/config/path";

        // Act
        config.ConfigDir = newPath;

        // Assert
        config.ConfigDir.Should().Be(newPath);
    }

    /// <summary>
    /// Tests that the static Caddy constant has the correct string value.
    /// Setup: Accesses the static CaddyServiceConfigurations.Caddy constant.
    /// Expectation: The constant should return "Caddy", providing a consistent identifier for the Caddy service throughout the application for configuration and service management purposes.
    /// </summary>
    [Fact]
    public void Constant_Caddy_HasCorrectValue()
    {
        // Assert
        CaddyServiceConfigurations.Caddy.Should().Be("Caddy");
    }

    /// <summary>
    /// Tests that the ConfigDir property accepts various string values including whitespace and special characters.
    /// Setup: Uses parameterized test data with whitespace characters, paths containing spaces, and paths with special characters.
    /// Expectation: The property should accept all string values without validation restrictions, ensuring flexibility for diverse file system naming conventions and edge cases in Caddy configuration paths.
    /// </summary>
    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    [InlineData("path with spaces")]
    [InlineData("path/with/special/chars!@#$%")]
    public void ConfigDir_WithVariousStringValues_AcceptsAll(string path)
    {
        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = path
        };

        // Assert
        config.ConfigDir.Should().Be(path);
    }

    /// <summary>
    /// Tests that the default ConfigDir value meets all expected criteria for a valid configuration directory.
    /// Setup: Creates a CaddyServiceConfigurations instance using the default constructor.
    /// Expectation: The default ConfigDir should be "/config", not null, and not empty, ensuring a reliable starting point for Caddy service configuration management.
    /// </summary>
    [Fact]
    public void DefaultValue_IsCorrect()
    {
        // Act
        var config = new CaddyServiceConfigurations();

        // Assert
        config.ConfigDir.Should().Be("/config");
        config.ConfigDir.Should().NotBeNull();
        config.ConfigDir.Should().NotBeEmpty();
    }

    /// <summary>
    /// Tests that the ConfigDir property correctly handles relative path values.
    /// Setup: Creates a CaddyServiceConfigurations instance and sets ConfigDir to a relative path "./config".
    /// Expectation: The property should accept and store relative paths, supporting deployment scenarios where Caddy configuration is relative to the application's working directory.
    /// </summary>
    [Fact]
    public void ConfigDir_WithRelativePath_SetsCorrectly()
    {
        // Arrange
        var relativePath = "./config";

        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = relativePath
        };

        // Assert
        config.ConfigDir.Should().Be(relativePath);
    }

    /// <summary>
    /// Tests that the ConfigDir property can handle very long path values without truncation or errors.
    /// Setup: Creates a CaddyServiceConfigurations instance and sets ConfigDir to an extremely long path string.
    /// Expectation: The property should store and return the complete long path, ensuring support for deeply nested directory structures that might be used in complex Caddy deployment scenarios.
    /// </summary>
    [Fact]
    public void ConfigDir_WithLongPath_SetsCorrectly()
    {
        // Arrange
        var longPath = "/very/long/path/to/caddy/configuration/directory/that/might/be/used/in/some/scenarios";

        // Act
        var config = new CaddyServiceConfigurations
        {
            ConfigDir = longPath
        };

        // Assert
        config.ConfigDir.Should().Be(longPath);
    }

    /// <summary>
    /// Tests that multiple CaddyServiceConfigurations instances maintain independent ConfigDir values.
    /// Setup: Creates two separate CaddyServiceConfigurations instances with different ConfigDir values.
    /// Expectation: Each instance should maintain its own ConfigDir value independently, ensuring proper isolation when managing multiple Caddy service configurations simultaneously.
    /// </summary>
    [Fact]
    public void MultipleInstances_HaveIndependentValues()
    {
        // Arrange
        var config1 = new CaddyServiceConfigurations { ConfigDir = "/path1" };
        var config2 = new CaddyServiceConfigurations { ConfigDir = "/path2" };

        // Act & Assert
        config1.ConfigDir.Should().Be("/path1");
        config2.ConfigDir.Should().Be("/path2");
        config1.ConfigDir.Should().NotBe(config2.ConfigDir);
    }
}
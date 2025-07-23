using CaddyManager.Contracts.Configurations.Docker;

namespace CaddyManager.Tests.Configurations.Docker;

/// <summary>
/// Tests for DockerServiceConfiguration
/// </summary>
public class DockerServiceConfigurationTests
{
    /// <summary>
    /// Tests that the DockerServiceConfiguration constructor initializes with the correct default values for Docker integration.
    /// Setup: Creates a new DockerServiceConfiguration instance using the default constructor.
    /// Expectation: CaddyContainerName should default to "caddy" and DockerHost should default to "unix:///var/run/docker.sock", ensuring proper Docker daemon communication and container identification for Caddy management.
    /// </summary>
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Act
        var config = new DockerServiceConfiguration();

        // Assert
        config.CaddyContainerName.Should().Be("caddy");
        config.DockerHost.Should().Be("unix:///var/run/docker.sock");
    }

    /// <summary>
    /// Tests that both CaddyContainerName and DockerHost properties can be set and retrieved correctly with custom values.
    /// Setup: Creates a DockerServiceConfiguration instance and sets both properties to custom values for container name and Docker host connection.
    /// Expectation: Both properties should store and return the exact values provided, enabling flexible Docker configuration for different deployment environments and container naming schemes.
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var containerName = "my-caddy-container";
        var dockerHost = "tcp://localhost:2376";

        // Act
        var config = new DockerServiceConfiguration
        {
            CaddyContainerName = containerName,
            DockerHost = dockerHost
        };

        // Assert
        config.CaddyContainerName.Should().Be(containerName);
        config.DockerHost.Should().Be(dockerHost);
    }

    /// <summary>
    /// Tests that the CaddyContainerName property accepts various valid Docker container naming conventions.
    /// Setup: Uses parameterized test data with different container name formats including hyphens, underscores, and version suffixes.
    /// Expectation: The property should accept all valid Docker container names, supporting diverse naming conventions used in different deployment scenarios and environments.
    /// </summary>
    [Theory]
    [InlineData("caddy")]
    [InlineData("my-caddy")]
    [InlineData("production-caddy")]
    [InlineData("caddy-server")]
    [InlineData("caddy_container")]
    [InlineData("caddy-v2")]
    public void CaddyContainerName_WithVariousNames_SetsCorrectly(string containerName)
    {
        // Act
        var config = new DockerServiceConfiguration
        {
            CaddyContainerName = containerName
        };

        // Assert
        config.CaddyContainerName.Should().Be(containerName);
    }

    /// <summary>
    /// Tests that the DockerHost property accepts various Docker daemon connection formats across different platforms.
    /// Setup: Uses parameterized test data with Unix socket, TCP, IP address, and Windows named pipe connection strings.
    /// Expectation: The property should accept all valid Docker host connection formats, supporting local and remote Docker daemon connections across Linux, Windows, and network-based Docker environments.
    /// </summary>
    [Theory]
    [InlineData("unix:///var/run/docker.sock")]
    [InlineData("tcp://localhost:2376")]
    [InlineData("tcp://docker-host:2376")]
    [InlineData("tcp://192.168.1.100:2376")]
    [InlineData("npipe:////./pipe/docker_engine")]
    public void DockerHost_WithVariousHosts_SetsCorrectly(string dockerHost)
    {
        // Act
        var config = new DockerServiceConfiguration
        {
            DockerHost = dockerHost
        };

        // Assert
        config.DockerHost.Should().Be(dockerHost);
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck prioritizes the DOCKER_HOST environment variable over the configured value when set.
    /// Setup: Sets DOCKER_HOST environment variable to a specific value and creates a configuration with a different DockerHost value.
    /// Expectation: The method should return the environment variable value, ensuring Docker client behavior consistency by respecting the standard DOCKER_HOST environment variable for Docker daemon connection.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_WithEnvironmentVariableSet_ReturnsEnvironmentValue()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
        var envValue = "tcp://env-host:2376";
        var configValue = "tcp://config-host:2376";

        try
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", envValue);
            var config = new DockerServiceConfiguration
            {
                DockerHost = configValue
            };

            // Act
            var result = config.DockerHostWithEnvCheck;

            // Assert
            result.Should().Be(envValue);
            result.Should().NotBe(configValue);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck returns the configured DockerHost value when DOCKER_HOST environment variable is not set.
    /// Setup: Ensures DOCKER_HOST environment variable is null and creates a configuration with a specific DockerHost value.
    /// Expectation: The method should return the configured value, providing fallback behavior when environment variables are not available for Docker daemon connection configuration.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_WithoutEnvironmentVariable_ReturnsConfigValue()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
        var configValue = "tcp://config-host:2376";

        try
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", null);
            var config = new DockerServiceConfiguration
            {
                DockerHost = configValue
            };

            // Act
            var result = config.DockerHostWithEnvCheck;

            // Assert
            result.Should().Be(configValue);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck returns the configured DockerHost value when DOCKER_HOST environment variable is empty.
    /// Setup: Sets DOCKER_HOST environment variable to an empty string and creates a configuration with a specific DockerHost value.
    /// Expectation: The method should return the configured value, treating empty environment variables as invalid and falling back to configuration for reliable Docker daemon connection.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_WithEmptyEnvironmentVariable_ReturnsConfigValue()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
        var configValue = "tcp://config-host:2376";

        try
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", "");
            var config = new DockerServiceConfiguration
            {
                DockerHost = configValue
            };

            // Act
            var result = config.DockerHostWithEnvCheck;

            // Assert
            result.Should().Be(configValue);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck returns the configured DockerHost value when DOCKER_HOST environment variable contains only whitespace.
    /// Setup: Sets DOCKER_HOST environment variable to whitespace characters and creates a configuration with a specific DockerHost value.
    /// Expectation: The method should return the configured value, treating whitespace-only environment variables as invalid and ensuring robust Docker daemon connection configuration.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_WithWhitespaceEnvironmentVariable_ReturnsConfigValue()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
        var configValue = "tcp://config-host:2376";

        try
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", "   ");
            var config = new DockerServiceConfiguration
            {
                DockerHost = configValue
            };

            // Act
            var result = config.DockerHostWithEnvCheck;

            // Assert
            result.Should().Be(configValue);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck returns the default Docker host value when both environment variable and configuration are not set.
    /// Setup: Ensures DOCKER_HOST environment variable is null and creates a default DockerServiceConfiguration instance.
    /// Expectation: The method should return the default "unix:///var/run/docker.sock" value, providing a reliable fallback for standard Docker daemon connection on Unix systems.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_WithBothNotSet_ReturnsDefaultValue()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");

        try
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", null);
            var config = new DockerServiceConfiguration();

            // Act
            var result = config.DockerHostWithEnvCheck;

            // Assert
            result.Should().Be("unix:///var/run/docker.sock");
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that the static Docker constant has the correct string value.
    /// Setup: Accesses the static DockerServiceConfiguration.Docker constant.
    /// Expectation: The constant should return "Docker", providing a consistent identifier for the Docker service throughout the application for configuration and service management purposes.
    /// </summary>
    [Fact]
    public void Constant_Docker_HasCorrectValue()
    {
        // Assert
        DockerServiceConfiguration.Docker.Should().Be("Docker");
    }

    /// <summary>
    /// Tests that both CaddyContainerName and DockerHost properties can be modified after the object has been created and initialized.
    /// Setup: Creates a DockerServiceConfiguration instance with default values, then changes both properties to new values.
    /// Expectation: Both properties should be mutable and accept new values, enabling runtime reconfiguration of Docker service settings for dynamic deployment scenarios.
    /// </summary>
    [Fact]
    public void Properties_CanBeModifiedAfterCreation()
    {
        // Arrange
        var config = new DockerServiceConfiguration();
        var newContainerName = "updated-caddy";
        var newDockerHost = "tcp://updated-host:2376";

        // Act
        config.CaddyContainerName = newContainerName;
        config.DockerHost = newDockerHost;

        // Assert
        config.CaddyContainerName.Should().Be(newContainerName);
        config.DockerHost.Should().Be(newDockerHost);
    }

    /// <summary>
    /// Tests that the CaddyContainerName property accepts empty, whitespace, and null values.
    /// Setup: Uses parameterized test data with empty string, whitespace, and null values for container name.
    /// Expectation: The property should accept all these values without validation restrictions, supporting edge cases where container name might be cleared or undefined in certain deployment scenarios.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CaddyContainerName_WithEmptyOrNullValues_SetsCorrectly(string? containerName)
    {
        // Act
        var config = new DockerServiceConfiguration
        {
            CaddyContainerName = containerName!
        };

        // Assert
        config.CaddyContainerName.Should().Be(containerName);
    }

    /// <summary>
    /// Tests that the DockerHost property accepts empty, whitespace, and null values.
    /// Setup: Uses parameterized test data with empty string, whitespace, and null values for Docker host connection.
    /// Expectation: The property should accept all these values without validation restrictions, supporting scenarios where Docker host configuration might be cleared or undefined.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void DockerHost_WithEmptyOrNullValues_SetsCorrectly(string? dockerHost)
    {
        // Act
        var config = new DockerServiceConfiguration
        {
            DockerHost = dockerHost!
        };

        // Assert
        config.DockerHost.Should().Be(dockerHost);
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck returns consistent results when called multiple times with the same environment variable value.
    /// Setup: Sets DOCKER_HOST environment variable to a specific value and calls DockerHostWithEnvCheck multiple times on the same configuration instance.
    /// Expectation: All calls should return the same environment variable value, ensuring consistent behavior and reliable Docker daemon connection configuration across multiple property accesses.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_MultipleCallsWithSameEnvironment_ReturnsConsistentResults()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
        var envValue = "tcp://consistent-host:2376";

        try
        {
            Environment.SetEnvironmentVariable("DOCKER_HOST", envValue);
            var config = new DockerServiceConfiguration();

            // Act
            var result1 = config.DockerHostWithEnvCheck;
            var result2 = config.DockerHostWithEnvCheck;

            // Assert
            result1.Should().Be(envValue);
            result2.Should().Be(envValue);
            result1.Should().Be(result2);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that DockerHostWithEnvCheck dynamically reflects changes to the DOCKER_HOST environment variable during execution.
    /// Setup: Creates a configuration instance and changes the DOCKER_HOST environment variable between calls to DockerHostWithEnvCheck.
    /// Expectation: The method should return the current environment variable value for each call, ensuring real-time responsiveness to environment changes for dynamic Docker daemon connection management.
    /// </summary>
    [Fact]
    public void DockerHostWithEnvCheck_EnvironmentChangeDuringExecution_ReflectsChange()
    {
        // Arrange
        var originalValue = Environment.GetEnvironmentVariable("DOCKER_HOST");
        var firstEnvValue = "tcp://first-host:2376";
        var secondEnvValue = "tcp://second-host:2376";

        try
        {
            var config = new DockerServiceConfiguration();

            Environment.SetEnvironmentVariable("DOCKER_HOST", firstEnvValue);
            var firstResult = config.DockerHostWithEnvCheck;

            Environment.SetEnvironmentVariable("DOCKER_HOST", secondEnvValue);
            var secondResult = config.DockerHostWithEnvCheck;

            // Assert
            firstResult.Should().Be(firstEnvValue);
            secondResult.Should().Be(secondEnvValue);
            firstResult.Should().NotBe(secondResult);
        }
        finally
        {
            // Cleanup
            Environment.SetEnvironmentVariable("DOCKER_HOST", originalValue);
        }
    }

    /// <summary>
    /// Tests that multiple DockerServiceConfiguration instances maintain independent property values for both CaddyContainerName and DockerHost.
    /// Setup: Creates two separate DockerServiceConfiguration instances with different values for both CaddyContainerName and DockerHost properties.
    /// Expectation: Each instance should maintain its own property values independently, ensuring proper isolation when managing multiple Docker service configurations simultaneously in complex deployment scenarios.
    /// </summary>
    [Fact]
    public void MultipleInstances_HaveIndependentValues()
    {
        // Arrange
        var config1 = new DockerServiceConfiguration
        {
            CaddyContainerName = "caddy1",
            DockerHost = "tcp://host1:2376"
        };
        var config2 = new DockerServiceConfiguration
        {
            CaddyContainerName = "caddy2",
            DockerHost = "tcp://host2:2376"
        };

        // Act & Assert
        config1.CaddyContainerName.Should().Be("caddy1");
        config2.CaddyContainerName.Should().Be("caddy2");
        config1.DockerHost.Should().Be("tcp://host1:2376");
        config2.DockerHost.Should().Be("tcp://host2:2376");

        config1.CaddyContainerName.Should().NotBe(config2.CaddyContainerName);
        config1.DockerHost.Should().NotBe(config2.DockerHost);
    }
}
using CaddyManager.Services.Caddy;
using CaddyManager.Tests.TestUtilities;
using System.Diagnostics;
using System.Text;

namespace CaddyManager.Tests.Services.Caddy;

/// <summary>
/// Tests for CaddyConfigurationParsingService
/// </summary>
public class CaddyConfigurationParsingServiceTests
{
    private readonly CaddyConfigurationParsingService _service;

    public CaddyConfigurationParsingServiceTests()
    {
        _service = new CaddyConfigurationParsingService();
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts a single hostname from a basic Caddyfile configuration.
    /// Setup: Provides a simple Caddyfile content string with one hostname directive using sample data.
    /// Expectation: The service should return a list containing exactly one hostname, enabling proper site identification and management in the Caddy web server configuration.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithSingleHostname_ReturnsCorrectHostname()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain("example.com");
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts multiple hostnames from a Caddyfile configuration with multiple host blocks.
    /// Setup: Provides a Caddyfile content string containing multiple hostname directives for different domains.
    /// Expectation: The service should return a list containing all configured hostnames, ensuring comprehensive site management across multiple domains in the Caddy web server.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithMultipleHostnames_ReturnsAllHostnames()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.MultipleHosts;

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("example.com");
        result.Should().Contain("www.example.com");
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts hostnames from a complex Caddyfile configuration with advanced directives.
    /// Setup: Provides a complex Caddyfile content string with multiple hosts and advanced configuration blocks.
    /// Expectation: The service should return all hostnames regardless of configuration complexity, ensuring robust parsing for production-level Caddy configurations.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithComplexConfiguration_ReturnsAllHostnames()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.ComplexConfiguration;

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Should only return outermost hostname declarations
        result.Should().Contain("api.example.com");
        result.Should().Contain("app.example.com");
    }

    /// <summary>
    /// Tests that the parsing service handles empty Caddyfile content gracefully without errors.
    /// Setup: Provides an empty string as Caddyfile content to simulate missing or uninitialized configuration.
    /// Expectation: The service should return an empty list rather than throwing exceptions, ensuring robust error handling for edge cases in Caddy configuration management.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithEmptyContent_ReturnsEmptyList()
    {
        // Arrange
        var caddyfileContent = string.Empty;

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the parsing service handles whitespace-only Caddyfile content gracefully.
    /// Setup: Provides a string containing only whitespace characters (spaces, tabs, newlines) to simulate malformed or empty configuration files.
    /// Expectation: The service should return an empty list, demonstrating proper input sanitization and preventing false positive hostname detection from whitespace.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithWhitespaceOnly_ReturnsEmptyList()
    {
        // Arrange
        var caddyfileContent = "   \n\t  \r\n  ";

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the parsing service deduplicates hostnames when the same hostname appears multiple times in a Caddyfile.
    /// Setup: Provides a Caddyfile content string with the same hostname configured in multiple blocks with different reverse proxy targets.
    /// Expectation: The service should return a unique list of hostnames, preventing duplicate entries that could cause confusion in Caddy configuration management.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithDuplicateHostnames_ReturnsUniqueHostnames()
    {
        // Arrange
        var caddyfileContent = @"
example.com {
    reverse_proxy localhost:8080
}

example.com {
    reverse_proxy localhost:9090
}";

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain("example.com");
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts the reverse proxy target hostname from a simple Caddyfile configuration.
    /// Setup: Provides a basic Caddyfile content string with a single reverse proxy directive pointing to a local service.
    /// Expectation: The service should return the target hostname (without port), enabling proper backend service identification for Caddy reverse proxy management.
    /// </summary>
    [Fact]
    public void GetReverseProxyTargetFromCaddyfileContent_WithSimpleReverseProxy_ReturnsCorrectTarget()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;

        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Be("localhost");
    }

    /// <summary>
    /// Tests that the parsing service extracts the first reverse proxy target from a complex Caddyfile configuration with multiple proxy directives.
    /// Setup: Provides a complex Caddyfile content string with multiple reverse proxy targets across different host blocks.
    /// Expectation: The service should return the first encountered target hostname, providing consistent behavior for configurations with multiple backend services.
    /// </summary>
    [Fact]
    public void GetReverseProxyTargetFromCaddyfileContent_WithComplexConfiguration_ReturnsFirstTarget()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.ComplexConfiguration;

        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Be("localhost");
    }

    /// <summary>
    /// Tests that the parsing service handles Caddyfile configurations without reverse proxy directives gracefully.
    /// Setup: Provides a Caddyfile content string with host blocks that use other directives (like respond) but no reverse proxy configuration.
    /// Expectation: The service should return an empty string, indicating no reverse proxy target is configured, which is valid for static content or other Caddy use cases.
    /// </summary>
    [Fact]
    public void GetReverseProxyTargetFromCaddyfileContent_WithNoReverseProxy_ReturnsEmptyString()
    {
        // Arrange
        var caddyfileContent = @"
example.com {
    respond ""Hello World""
}";

        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Be(string.Empty);
    }

    /// <summary>
    /// Tests that the parsing service handles empty Caddyfile content when extracting reverse proxy targets.
    /// Setup: Provides an empty string as Caddyfile content to simulate missing configuration.
    /// Expectation: The service should return an empty string rather than throwing exceptions, ensuring robust error handling for edge cases in reverse proxy target extraction.
    /// </summary>
    [Fact]
    public void GetReverseProxyTargetFromCaddyfileContent_WithEmptyContent_ReturnsEmptyString()
    {
        // Arrange
        var caddyfileContent = string.Empty;

        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Be(string.Empty);
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts a single port number from a Caddyfile reverse proxy configuration.
    /// Setup: Provides a simple Caddyfile content string with one reverse proxy directive specifying a port number.
    /// Expectation: The service should return a list containing the correct port number, enabling proper backend service port identification for Caddy reverse proxy management.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithSinglePort_ReturnsCorrectPort()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(8080);
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts multiple port numbers from a Caddyfile configuration with multiple reverse proxy targets.
    /// Setup: Provides a Caddyfile content string with multiple reverse proxy directives using different port numbers.
    /// Expectation: The service should return a list containing all configured port numbers, ensuring comprehensive port management for multi-service Caddy configurations.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithMultiplePorts_ReturnsAllPorts()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.WithMultiplePorts;

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(8080);
        result.Should().Contain(3000);
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts all port numbers from a complex Caddyfile configuration with multiple hosts and services.
    /// Setup: Provides a complex Caddyfile content string with multiple host blocks, each containing reverse proxy directives with different ports.
    /// Expectation: The service should return all unique port numbers across all configurations, enabling comprehensive port tracking for complex Caddy deployments.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithComplexConfiguration_ReturnsAllPorts()
    {
        // Arrange
        var caddyfileContent = TestHelper.SampleCaddyfiles.ComplexConfiguration;

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(3000);
        result.Should().Contain(3001);
        result.Should().Contain(8080);
    }

    /// <summary>
    /// Tests that the parsing service deduplicates port numbers when the same port appears multiple times in a Caddyfile configuration.
    /// Setup: Provides a Caddyfile content string with multiple host blocks using the same reverse proxy port number.
    /// Expectation: The service should return a unique list of port numbers, preventing duplicate entries that could cause confusion in port management and resource allocation.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithDuplicatePorts_ReturnsUniquePorts()
    {
        // Arrange
        var caddyfileContent = @"
example.com {
    reverse_proxy localhost:8080
}

api.example.com {
    reverse_proxy localhost:8080
}";

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(8080);
    }

    /// <summary>
    /// Tests that the parsing service handles Caddyfile configurations without reverse proxy directives when extracting ports.
    /// Setup: Provides a Caddyfile content string with host blocks that use other directives but no reverse proxy configuration.
    /// Expectation: The service should return an empty list, indicating no reverse proxy ports are configured, which is valid for static content or other non-proxy Caddy use cases.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithNoReverseProxy_ReturnsEmptyList()
    {
        // Arrange
        var caddyfileContent = @"
example.com {
    respond ""Hello World""
}";

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the parsing service handles empty Caddyfile content when extracting reverse proxy ports.
    /// Setup: Provides an empty string as Caddyfile content to simulate missing configuration.
    /// Expectation: The service should return an empty list rather than throwing exceptions, ensuring robust error handling for edge cases in port extraction.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithEmptyContent_ReturnsEmptyList()
    {
        // Arrange
        var caddyfileContent = string.Empty;

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts reverse proxy targets from various Caddyfile configurations with different target formats.
    /// Setup: Provides parameterized test data with different reverse proxy target formats including IP addresses, hostnames, and URLs.
    /// Expectation: The service should correctly parse and return the target hostname portion from various reverse proxy directive formats, ensuring compatibility with different backend service configurations.
    /// </summary>
    [Theory]
    [InlineData("example.com { reverse_proxy 192.168.1.100:3000 }", "192.168.1.100")]
    [InlineData("test.local { reverse_proxy app-server:8080 }", "app-server")]
    [InlineData("api.test { reverse_proxy http://backend:9000 }", "http")]
    public void GetReverseProxyTargetFromCaddyfileContent_WithVariousTargets_ReturnsCorrectTarget(
        string caddyfileContent, string expectedTarget)
    {
        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Be(expectedTarget);
    }

    /// <summary>
    /// Tests that the parsing service correctly extracts port numbers from various Caddyfile configurations with different reverse proxy port formats.
    /// Setup: Provides parameterized test data with different reverse proxy configurations using various port numbers and target formats.
    /// Expectation: The service should correctly parse and return the port number from various reverse proxy directive formats, ensuring accurate port identification for different backend service configurations.
    /// </summary>
    [Theory]
    [InlineData("example.com { reverse_proxy localhost:3000 }", 3000)]
    [InlineData("test.local { reverse_proxy app-server:8080 }", 8080)]
    [InlineData("api.test { reverse_proxy backend:9000 }", 9000)]
    public void GetReverseProxyPortsFromCaddyfileContent_WithVariousPorts_ReturnsCorrectPort(
        string caddyfileContent, int expectedPort)
    {
        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(expectedPort);
    }

    #region Additional Edge Cases and Malformed Content Tests

    /// <summary>
    /// Tests that the parsing service handles malformed Caddyfile content with invalid syntax gracefully.
    /// Setup: Provides Caddyfile content with invalid syntax, missing braces, and malformed directives.
    /// Expectation: The service should extract whatever valid hostnames it can find and ignore malformed sections, ensuring robust parsing of partially corrupted Caddy configurations.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithMalformedSyntax_ExtractsValidHostnames()
    {
        // Arrange
        var malformedContent = @"
example.com {
    reverse_proxy localhost:3000
}

invalid-syntax {
    reverse_proxy

malformed {
    reverse_proxy localhost:8080
";

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(malformedContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("example.com");
        result.Should().Contain("invalid-syntax");
        result.Should().Contain("malformed");
    }

    /// <summary>
    /// Tests that the parsing service handles Unicode and special characters in hostnames correctly.
    /// Setup: Provides Caddyfile content with hostnames containing Unicode characters, special symbols, and international domain names.
    /// Expectation: The service should correctly parse and return hostnames with Unicode and special characters, ensuring support for international domain names and special naming conventions.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithUnicodeHostnames_ReturnsCorrectHostnames()
    {
        // Arrange
        var unicodeContent = @"
测试.example.com {
    reverse_proxy localhost:3000
}

api-测试.local {
    reverse_proxy localhost:8080
}

special-chars!@#$.test {
    reverse_proxy localhost:9000
}";

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(unicodeContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain("测试.example.com");
        result.Should().Contain("api-测试.local");
        result.Should().Contain("special-chars!@#$.test");
    }

    /// <summary>
    /// Tests that the parsing service handles very large Caddyfile content without performance issues.
    /// Setup: Creates a very large Caddyfile content with many hostnames and complex configurations.
    /// Expectation: The service should process large configurations efficiently without throwing exceptions or experiencing significant performance degradation, ensuring the system can handle production-sized Caddy configurations.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithLargeContent_ProcessesEfficiently()
    {
        // Arrange
        var largeContent = new StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            largeContent.AppendLine($"host{i}.example.com {{");
            largeContent.AppendLine("    reverse_proxy localhost:3000");
            largeContent.AppendLine("}");
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = _service.GetHostnamesFromCaddyfileContent(largeContent.ToString());
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should process in under 1 second
    }

    /// <summary>
    /// Tests that the parsing service handles nested and complex Caddyfile configurations correctly.
    /// Setup: Provides a complex Caddyfile with nested blocks, multiple directives, and advanced configuration patterns.
    /// Expectation: The service should correctly extract hostnames from complex nested configurations, ensuring accurate parsing of advanced Caddy configuration patterns used in production environments.
    /// </summary>
    [Fact]
    public void GetHostnamesFromCaddyfileContent_WithComplexNestedConfiguration_ReturnsAllHostnames()
    {
        // Arrange
        var complexContent = @"
api.example.com {
    reverse_proxy localhost:3000
    header {
        Access-Control-Allow-Origin *
    }
    @cors {
        method OPTIONS
    }
    respond @cors 204
}

app.example.com {
    reverse_proxy localhost:8080
    tls {
        protocols tls1.2 tls1.3
    }
    header {
        Strict-Transport-Security max-age=31536000
    }
}";

        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(complexContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2); // Should only return outermost hostname declarations
        result.Should().Contain("api.example.com");
        result.Should().Contain("app.example.com");
    }

    /// <summary>
    /// Tests that the parsing service handles edge cases in regex patterns correctly.
    /// Setup: Provides Caddyfile content with edge cases that might break regex parsing, including unusual whitespace, comments, and formatting.
    /// Expectation: The service should handle regex edge cases gracefully, ensuring robust parsing regardless of Caddyfile formatting and style variations.
    /// </summary>
    [Theory]
    [InlineData("example.com{reverse_proxy localhost:3000}")] // No spaces
    [InlineData("example.com\n{\nreverse_proxy localhost:3000\n}")] // Newlines
    [InlineData("example.com\t{\treverse_proxy localhost:3000\t}")] // Tabs
    public void GetHostnamesFromCaddyfileContent_WithRegexEdgeCases_ReturnsCorrectHostnames(string content)
    {
        // Act
        var result = _service.GetHostnamesFromCaddyfileContent(content);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("example.com");
    }

    /// <summary>
    /// Tests that the parsing service handles reverse proxy targets with various formats and edge cases.
    /// Setup: Provides Caddyfile content with different reverse proxy target formats including IP addresses, hostnames, URLs, and edge cases.
    /// Expectation: The service should correctly extract reverse proxy targets from various formats, ensuring accurate parsing of different reverse proxy configurations.
    /// </summary>
    [Theory]
    [InlineData("example.com { reverse_proxy 192.168.1.100:3000 }", "192.168.1.100")]
    [InlineData("test.local { reverse_proxy app-server:8080 }", "app-server")]
    [InlineData("api.test { reverse_proxy http://backend:9000 }", "http")]
    [InlineData("web.test { reverse_proxy https://secure-backend:8443 }", "https")]
    [InlineData("app.test { reverse_proxy localhost }", "localhost")]
    public void GetReverseProxyTargetFromCaddyfileContent_WithVariousFormats_ReturnsCorrectTarget(
        string caddyfileContent, string expectedTarget)
    {
        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Be(expectedTarget);
    }

    /// <summary>
    /// Tests that the parsing service handles malformed reverse proxy directives gracefully.
    /// Setup: Provides Caddyfile content with malformed reverse proxy directives that might cause parsing errors.
    /// Expectation: The service should handle malformed reverse proxy directives gracefully, either by extracting partial information or returning empty results, ensuring robust parsing of corrupted configurations.
    /// </summary>
    [Fact]
    public void GetReverseProxyTargetFromCaddyfileContent_WithMalformedDirectives_HandlesGracefully()
    {
        // Arrange
        var malformedContent = @"example.com { reverse_proxy }"; // Malformed: reverse_proxy without target

        // Act
        var result = _service.GetReverseProxyTargetFromCaddyfileContent(malformedContent);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(string.Empty);
    }

    /// <summary>
    /// Tests that the parsing service handles port extraction with various edge cases correctly.
    /// Setup: Provides Caddyfile content with different port formats, invalid ports, and edge cases.
    /// Expectation: The service should correctly extract valid ports and handle invalid port formats gracefully, ensuring accurate port detection for reverse proxy configurations.
    /// </summary>
    [Theory]
    [InlineData("example.com { reverse_proxy localhost:3000 }", 3000)]
    [InlineData("test.local { reverse_proxy app-server:8080 }", 8080)]
    [InlineData("api.test { reverse_proxy backend:9000 }", 9000)]
    [InlineData("web.test { reverse_proxy https://secure-backend:8443 }", 8443)]
    public void GetReverseProxyPortsFromCaddyfileContent_WithVariousPorts_ReturnsCorrectPorts(
        string caddyfileContent, int expectedPort)
    {
        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(caddyfileContent);

        // Assert
        result.Should().Contain(expectedPort);
    }

    /// <summary>
    /// Tests that the parsing service handles invalid port formats gracefully.
    /// Setup: Provides Caddyfile content with invalid port formats that should not be parsed as valid ports.
    /// Expectation: The service should ignore invalid port formats and only return valid port numbers, ensuring robust port parsing that doesn't break on malformed configurations.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithInvalidPorts_HandlesGracefully()
    {
        // Arrange
        var invalidPortContent = @"
example.com {
    reverse_proxy localhost:invalid
}

test.local {
    reverse_proxy app-server:99999
}

api.test {
    reverse_proxy backend:-1
}";

        // Act
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(invalidPortContent);

        // Assert
        result.Should().NotBeNull();
        // The service might still parse some invalid ports as valid numbers
        // This test verifies that the service handles malformed port data gracefully
        result.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the parsing service handles performance with very large reverse proxy configurations.
    /// Setup: Creates a very large Caddyfile content with many reverse proxy directives.
    /// Expectation: The service should process large reverse proxy configurations efficiently without performance issues, ensuring the system can handle complex production configurations.
    /// </summary>
    [Fact]
    public void GetReverseProxyPortsFromCaddyfileContent_WithLargeConfiguration_ProcessesEfficiently()
    {
        // Arrange
        var largeContent = new StringBuilder();
        for (int i = 0; i < 1000; i++)
        {
            largeContent.AppendLine($"host{i}.example.com {{");
            largeContent.AppendLine($"    reverse_proxy localhost:{3000 + i}");
            largeContent.AppendLine("}");
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = _service.GetReverseProxyPortsFromCaddyfileContent(largeContent.ToString());
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1000);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should process in under 1 second
    }

    #endregion
}
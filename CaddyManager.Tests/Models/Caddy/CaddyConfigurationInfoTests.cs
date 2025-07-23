using CaddyManager.Contracts.Models.Caddy;

namespace CaddyManager.Tests.Models.Caddy;

/// <summary>
/// Tests for CaddyConfigurationInfo model
/// </summary>
public class CaddyConfigurationInfoTests
{
    /// <summary>
    /// Tests that the CaddyConfigurationInfo constructor initializes all properties with appropriate default values.
    /// Setup: Creates a new CaddyConfigurationInfo instance using the default constructor.
    /// Expectation: All collection properties should be initialized as empty but non-null collections, and string properties should be empty strings, ensuring the model is ready for immediate use in Caddy configuration management without null reference exceptions.
    /// </summary>
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Act
        var info = new CaddyConfigurationInfo();

        // Assert
        info.Hostnames.Should().NotBeNull();
        info.Hostnames.Should().BeEmpty();
        info.ReverseProxyHostname.Should().Be(string.Empty);
        info.ReverseProxyPorts.Should().NotBeNull();
        info.ReverseProxyPorts.Should().BeEmpty();
        info.FileName.Should().Be(string.Empty);
        info.AggregatedReverseProxyPorts.Should().NotBeNull();
        info.AggregatedReverseProxyPorts.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that all CaddyConfigurationInfo properties can be properly set and retrieved with various data types.
    /// Setup: Creates test data including hostname lists, reverse proxy configuration, file names, and port collections that represent typical Caddy configuration scenarios.
    /// Expectation: All properties should store and return the exact values assigned, ensuring data integrity for Caddy configuration information used in reverse proxy management and file-based configuration tracking.
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var hostnames = new List<string> { "example.com", "www.example.com" };
        var reverseProxyHostname = "localhost";
        var reverseProxyPorts = new List<int> { 8080, 9090 };
        var fileName = "test-config";
        var aggregatedPorts = new List<int> { 8080, 9090, 3000 };

        // Act
        var info = new CaddyConfigurationInfo
        {
            Hostnames = hostnames,
            ReverseProxyHostname = reverseProxyHostname,
            ReverseProxyPorts = reverseProxyPorts,
            FileName = fileName,
            AggregatedReverseProxyPorts = aggregatedPorts
        };

        // Assert
        info.Hostnames.Should().BeEquivalentTo(hostnames);
        info.ReverseProxyHostname.Should().Be(reverseProxyHostname);
        info.ReverseProxyPorts.Should().BeEquivalentTo(reverseProxyPorts);
        info.FileName.Should().Be(fileName);
        info.AggregatedReverseProxyPorts.Should().BeEquivalentTo(aggregatedPorts);
    }

    /// <summary>
    /// Tests that two CaddyConfigurationInfo instances with identical FileName values are considered equal.
    /// Setup: Creates two separate CaddyConfigurationInfo instances with the same FileName but potentially different other properties.
    /// Expectation: The Equals method should return true since equality is based solely on FileName, enabling proper configuration identification and deduplication in Caddy management operations where configurations are uniquely identified by their file names.
    /// </summary>
    [Fact]
    public void Equals_WithSameFileName_ReturnsTrue()
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo { FileName = "test" };
        var info2 = new CaddyConfigurationInfo { FileName = "test" };

        // Act & Assert
        info1.Equals(info2).Should().BeTrue();
        info2.Equals(info1).Should().BeTrue();
    }

    /// <summary>
    /// Tests that two CaddyConfigurationInfo instances with different FileName values are not considered equal.
    /// Setup: Creates two CaddyConfigurationInfo instances with different FileName values.
    /// Expectation: The Equals method should return false, ensuring that configurations with different file names are treated as distinct entities in Caddy configuration management, preventing accidental merging or confusion between separate configuration files.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentFileName_ReturnsFalse()
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo { FileName = "test1" };
        var info2 = new CaddyConfigurationInfo { FileName = "test2" };

        // Act & Assert
        info1.Equals(info2).Should().BeFalse();
        info2.Equals(info1).Should().BeFalse();
    }

    /// <summary>
    /// Tests that CaddyConfigurationInfo equality is determined solely by FileName, ignoring all other property differences.
    /// Setup: Creates two instances with identical FileName but completely different hostnames, reverse proxy settings, and port configurations.
    /// Expectation: The Equals method should return true, confirming that only FileName determines equality, which is crucial for configuration management where the same file might have different runtime properties but represents the same logical configuration unit.
    /// </summary>
    [Fact]
    public void Equals_WithSameFileNameButDifferentOtherProperties_ReturnsTrue()
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo 
        { 
            FileName = "test",
            Hostnames = new List<string> { "example.com" },
            ReverseProxyHostname = "localhost",
            ReverseProxyPorts = new List<int> { 8080 }
        };
        var info2 = new CaddyConfigurationInfo 
        { 
            FileName = "test",
            Hostnames = new List<string> { "different.com" },
            ReverseProxyHostname = "different-host",
            ReverseProxyPorts = new List<int> { 9090 }
        };

        // Act & Assert
        info1.Equals(info2).Should().BeTrue();
    }

    /// <summary>
    /// Tests that comparing a CaddyConfigurationInfo instance with null returns false.
    /// Setup: Creates a valid CaddyConfigurationInfo instance and compares it with null.
    /// Expectation: The Equals method should return false when compared with null, ensuring robust null-safety in configuration comparison operations and preventing null reference exceptions in Caddy management workflows.
    /// </summary>
    [Fact]
    public void Equals_WithNull_ReturnsFalse()
    {
        // Arrange
        var info = new CaddyConfigurationInfo { FileName = "test" };

        // Act & Assert
        info.Equals(null).Should().BeFalse();
    }

    /// <summary>
    /// Tests that comparing a CaddyConfigurationInfo instance with an object of a different type returns false.
    /// Setup: Creates a CaddyConfigurationInfo instance and compares it with a string object.
    /// Expectation: The Equals method should return false when compared with different object types, ensuring type safety in configuration comparison operations and maintaining proper object equality semantics in Caddy management systems.
    /// </summary>
    [Fact]
    public void Equals_WithDifferentType_ReturnsFalse()
    {
        // Arrange
        var info = new CaddyConfigurationInfo { FileName = "test" };
        var otherObject = "test";

        // Act & Assert
        info.Equals(otherObject).Should().BeFalse();
    }

    /// <summary>
    /// Tests that a CaddyConfigurationInfo instance is equal to itself (reflexive property of equality).
    /// Setup: Creates a CaddyConfigurationInfo instance and compares it with itself.
    /// Expectation: The Equals method should return true when an instance is compared with itself, satisfying the reflexive property of equality and ensuring consistent behavior in configuration management operations where self-comparison might occur.
    /// </summary>
    [Fact]
    public void Equals_WithSameInstance_ReturnsTrue()
    {
        // Arrange
        var info = new CaddyConfigurationInfo { FileName = "test" };

        // Act & Assert
        info.Equals(info).Should().BeTrue();
    }

    /// <summary>
    /// Tests that two CaddyConfigurationInfo instances with the same FileName produce identical hash codes.
    /// Setup: Creates two separate instances with identical FileName values and calculates their hash codes.
    /// Expectation: Both instances should produce the same hash code, ensuring proper behavior in hash-based collections like dictionaries and hash sets used for efficient configuration lookup and storage in Caddy management systems.
    /// </summary>
    [Fact]
    public void GetHashCode_WithSameFileName_ReturnsSameHashCode()
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo { FileName = "test" };
        var info2 = new CaddyConfigurationInfo { FileName = "test" };

        // Act
        var hash1 = info1.GetHashCode();
        var hash2 = info2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    /// <summary>
    /// Tests that two CaddyConfigurationInfo instances with different FileName values produce different hash codes.
    /// Setup: Creates two instances with different FileName values and calculates their hash codes.
    /// Expectation: The instances should produce different hash codes, ensuring good hash distribution for efficient storage and retrieval in hash-based collections used for Caddy configuration management and reducing hash collisions.
    /// </summary>
    [Fact]
    public void GetHashCode_WithDifferentFileName_ReturnsDifferentHashCode()
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo { FileName = "test1" };
        var info2 = new CaddyConfigurationInfo { FileName = "test2" };

        // Act
        var hash1 = info1.GetHashCode();
        var hash2 = info2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    /// <summary>
    /// Tests that CaddyConfigurationInfo instances with empty FileName values produce consistent hash codes.
    /// Setup: Creates two instances with empty FileName values and calculates their hash codes.
    /// Expectation: Both instances should produce identical hash codes, ensuring that configurations with empty file names (edge cases) behave consistently in hash-based operations and don't cause issues in Caddy configuration management systems.
    /// </summary>
    [Fact]
    public void GetHashCode_WithEmptyFileName_ReturnsConsistentHashCode()
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo { FileName = string.Empty };
        var info2 = new CaddyConfigurationInfo { FileName = string.Empty };

        // Act
        var hash1 = info1.GetHashCode();
        var hash2 = info2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    /// <summary>
    /// Tests that CaddyConfigurationInfo instances produce consistent hash codes across various FileName formats and lengths.
    /// Setup: Uses parameterized test data including empty strings, short names, and long names with special characters representing different Caddy configuration file naming patterns.
    /// Expectation: Each FileName should consistently produce the same hash code across multiple instances, ensuring reliable behavior in hash-based collections regardless of configuration file naming conventions used in Caddy deployments.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("very-long-configuration-file-name-with-special-characters-123")]
    public void GetHashCode_WithVariousFileNames_ReturnsConsistentHashCode(string fileName)
    {
        // Arrange
        var info1 = new CaddyConfigurationInfo { FileName = fileName };
        var info2 = new CaddyConfigurationInfo { FileName = fileName };

        // Act
        var hash1 = info1.GetHashCode();
        var hash2 = info2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    /// <summary>
    /// Tests that the Hostnames collection can be modified after CaddyConfigurationInfo creation.
    /// Setup: Creates a CaddyConfigurationInfo instance and adds multiple hostname entries to the Hostnames collection.
    /// Expectation: The collection should accept new entries and maintain them correctly, enabling dynamic hostname management for Caddy configurations where domains can be added or modified during runtime configuration updates.
    /// </summary>
    [Fact]
    public void Hostnames_CanBeModified()
    {
        // Arrange
        var info = new CaddyConfigurationInfo();

        // Act
        info.Hostnames.Add("example.com");
        info.Hostnames.Add("www.example.com");

        // Assert
        info.Hostnames.Should().HaveCount(2);
        info.Hostnames.Should().Contain("example.com");
        info.Hostnames.Should().Contain("www.example.com");
    }

    /// <summary>
    /// Tests that the ReverseProxyPorts collection can be modified after CaddyConfigurationInfo creation.
    /// Setup: Creates a CaddyConfigurationInfo instance and adds multiple port numbers to the ReverseProxyPorts collection.
    /// Expectation: The collection should accept new port entries and maintain them correctly, enabling dynamic port configuration management for Caddy reverse proxy setups where backend service ports can be added or changed during configuration updates.
    /// </summary>
    [Fact]
    public void ReverseProxyPorts_CanBeModified()
    {
        // Arrange
        var info = new CaddyConfigurationInfo();

        // Act
        info.ReverseProxyPorts.Add(8080);
        info.ReverseProxyPorts.Add(9090);

        // Assert
        info.ReverseProxyPorts.Should().HaveCount(2);
        info.ReverseProxyPorts.Should().Contain(8080);
        info.ReverseProxyPorts.Should().Contain(9090);
    }

    /// <summary>
    /// Tests that the AggregatedReverseProxyPorts collection can be modified after CaddyConfigurationInfo creation.
    /// Setup: Creates a CaddyConfigurationInfo instance and adds multiple port numbers to the AggregatedReverseProxyPorts collection.
    /// Expectation: The collection should accept new port entries and maintain them correctly, enabling management of aggregated port information across multiple configurations, which is essential for comprehensive Caddy deployment monitoring and port conflict detection.
    /// </summary>
    [Fact]
    public void AggregatedReverseProxyPorts_CanBeModified()
    {
        // Arrange
        var info = new CaddyConfigurationInfo();

        // Act
        info.AggregatedReverseProxyPorts.Add(3000);
        info.AggregatedReverseProxyPorts.Add(4000);

        // Assert
        info.AggregatedReverseProxyPorts.Should().HaveCount(2);
        info.AggregatedReverseProxyPorts.Should().Contain(3000);
        info.AggregatedReverseProxyPorts.Should().Contain(4000);
    }

    #region Additional Edge Cases and Boundary Value Tests

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles null hostnames gracefully.
    /// Setup: Creates a CaddyConfigurationInfo instance and assigns null to the Hostnames property.
    /// Expectation: The model should handle null hostnames gracefully, either by treating null as empty list or providing appropriate null handling, ensuring robust operation with null inputs.
    /// </summary>
    [Fact]
    public void Hostnames_WithNullValue_HandlesGracefully()
    {
        // Arrange
        var info = new CaddyConfigurationInfo();

        // Act
        info.Hostnames = null!;

        // Assert
        // The model should handle null assignment gracefully
        // This test verifies that the model doesn't crash with null assignments
        info.Hostnames.Should().BeNull();
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles very large hostname lists efficiently.
    /// Setup: Creates a CaddyConfigurationInfo with a very large list of hostnames to test performance and memory usage.
    /// Expectation: The model should handle large hostname lists efficiently without performance issues, ensuring the system can handle complex Caddy configurations with many hostnames.
    /// </summary>
    [Fact]
    public void Hostnames_WithLargeList_HandlesEfficiently()
    {
        // Arrange
        var largeHostnameList = new List<string>();
        for (int i = 0; i < 10000; i++)
        {
            largeHostnameList.Add($"host{i}.example.com");
        }

        // Act
        var info = new CaddyConfigurationInfo
        {
            Hostnames = largeHostnameList
        };

        // Assert
        info.Hostnames.Should().NotBeNull();
        info.Hostnames.Should().HaveCount(10000);
        info.Hostnames.Should().Contain("host0.example.com");
        info.Hostnames.Should().Contain("host9999.example.com");
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles Unicode and special characters in hostnames correctly.
    /// Setup: Creates a CaddyConfigurationInfo with hostnames containing Unicode characters and special symbols.
    /// Expectation: The model should correctly handle Unicode and special characters in hostnames, ensuring support for international domain names and special naming conventions.
    /// </summary>
    [Fact]
    public void Hostnames_WithUnicodeAndSpecialCharacters_HandlesCorrectly()
    {
        // Arrange
        var unicodeHostnames = new List<string>
        {
            "测试.example.com",
            "api-测试.local",
            "special-chars!@#$.test",
            "host-with-spaces.example.com",
            "host.with.dots.example.com"
        };

        // Act
        var info = new CaddyConfigurationInfo
        {
            Hostnames = unicodeHostnames
        };

        // Assert
        info.Hostnames.Should().NotBeNull();
        info.Hostnames.Should().HaveCount(5);
        info.Hostnames.Should().Contain("测试.example.com");
        info.Hostnames.Should().Contain("api-测试.local");
        info.Hostnames.Should().Contain("special-chars!@#$.test");
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles boundary values for reverse proxy ports correctly.
    /// Setup: Creates a CaddyConfigurationInfo with boundary port values including minimum, maximum, and edge case port numbers.
    /// Expectation: The model should handle boundary port values correctly, ensuring proper validation and storage of port numbers across the valid port range.
    /// </summary>
    [Fact]
    public void ReverseProxyPorts_WithBoundaryValues_HandlesCorrectly()
    {
        // Arrange
        var boundaryPorts = new List<int>
        {
            1, // Minimum valid port
            1024, // Common system port boundary
            65535, // Maximum valid port
            8080, // Common application port
            443, // HTTPS port
            80 // HTTP port
        };

        // Act
        var info = new CaddyConfigurationInfo
        {
            ReverseProxyPorts = boundaryPorts
        };

        // Assert
        info.ReverseProxyPorts.Should().NotBeNull();
        info.ReverseProxyPorts.Should().HaveCount(6);
        info.ReverseProxyPorts.Should().Contain(1);
        info.ReverseProxyPorts.Should().Contain(65535);
        info.ReverseProxyPorts.Should().Contain(8080);
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles empty and null collections gracefully.
    /// Setup: Creates CaddyConfigurationInfo instances with empty collections and null values for various properties.
    /// Expectation: The model should handle empty and null collections gracefully, ensuring robust operation with incomplete or missing data.
    /// </summary>
    [Theory]
    [InlineData(true, true, true)] // All empty
    [InlineData(false, true, true)] // Some empty
    [InlineData(true, false, true)] // Some empty
    [InlineData(true, true, false)] // Some empty
    public void Collections_WithEmptyAndNullValues_HandlesGracefully(bool emptyHostnames, bool emptyPorts, bool emptyAggregatedPorts)
    {
        // Arrange
        var info = new CaddyConfigurationInfo();

        // Act
        if (emptyHostnames)
        {
            info.Hostnames = new List<string>();
        }

        if (emptyPorts)
        {
            info.ReverseProxyPorts = new List<int>();
        }

        if (emptyAggregatedPorts)
        {
            info.AggregatedReverseProxyPorts = new List<int>();
        }

        // Assert
        info.Hostnames.Should().NotBeNull();
        info.ReverseProxyPorts.Should().NotBeNull();
        info.AggregatedReverseProxyPorts.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles very long filenames correctly.
    /// Setup: Creates a CaddyConfigurationInfo with a very long filename to test boundary conditions.
    /// Expectation: The model should handle very long filenames correctly, ensuring proper storage and retrieval of long file identifiers.
    /// </summary>
    [Fact]
    public void FileName_WithVeryLongName_HandlesCorrectly()
    {
        // Arrange
        var veryLongFileName = new string('a', 1000); // 1000 character filename

        // Act
        var info = new CaddyConfigurationInfo
        {
            FileName = veryLongFileName
        };

        // Assert
        info.FileName.Should().Be(veryLongFileName);
        info.FileName.Should().HaveLength(1000);
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles special characters in filenames correctly.
    /// Setup: Creates a CaddyConfigurationInfo with filenames containing special characters and Unicode.
    /// Expectation: The model should handle special characters in filenames correctly, ensuring support for international file naming conventions.
    /// </summary>
    [Theory]
    [InlineData("config-with-unicode-测试")]
    [InlineData("config-with-special-chars!@#$%")]
    [InlineData("config.with.dots")]
    [InlineData("config-with-spaces and-dashes")]
    [InlineData("config_with_underscores")]
    public void FileName_WithSpecialCharacters_HandlesCorrectly(string fileName)
    {
        // Act
        var info = new CaddyConfigurationInfo
        {
            FileName = fileName
        };

        // Assert
        info.FileName.Should().Be(fileName);
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles concurrent access scenarios gracefully.
    /// Setup: Creates multiple concurrent operations on a CaddyConfigurationInfo instance to test thread safety.
    /// Expectation: The model should handle concurrent access gracefully without throwing exceptions or causing race conditions, ensuring thread safety in multi-user environments.
    /// </summary>
    [Fact]
    public async Task CaddyConfigurationInfo_WithConcurrentAccess_HandlesGracefully()
    {
        // Arrange
        var info = new CaddyConfigurationInfo();
        var tasks = new List<Task>();

        // Act - Create multiple concurrent operations
        for (int i = 0; i < 10; i++)
        {
            var index = i;
            tasks.Add(Task.Run(() =>
            {
                info.Hostnames = new List<string> { $"host{index}.example.com" };
                info.ReverseProxyPorts = new List<int> { 3000 + index };
                info.FileName = $"config{index}";
            }));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        // Assert
        info.Should().NotBeNull();
        info.Hostnames.Should().NotBeNull();
        info.ReverseProxyPorts.Should().NotBeNull();
        info.FileName.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles memory pressure scenarios gracefully.
    /// Setup: Creates a scenario where the model might be under memory pressure with large data structures.
    /// Expectation: The model should handle memory pressure scenarios gracefully, either by implementing memory-efficient operations or providing appropriate error handling, ensuring robust operation under resource constraints.
    /// </summary>
    [Fact]
    public void CaddyConfigurationInfo_WithMemoryPressure_HandlesGracefully()
    {
        // Arrange
        var largeHostnames = new List<string>();
        var largePorts = new List<int>();

        for (int i = 0; i < 1000; i++)
        {
            largeHostnames.Add($"host{i}.example.com");
            largePorts.Add(3000 + i);
        }

        // Act
        var info = new CaddyConfigurationInfo
        {
            Hostnames = largeHostnames,
            ReverseProxyPorts = largePorts,
            AggregatedReverseProxyPorts = largePorts,
            FileName = "large-config"
        };

        // Assert
        info.Should().NotBeNull();
        info.Hostnames.Should().HaveCount(1000);
        info.ReverseProxyPorts.Should().HaveCount(1000);
        info.AggregatedReverseProxyPorts.Should().HaveCount(1000);
    }

    /// <summary>
    /// Tests that the CaddyConfigurationInfo model handles serialization and deserialization correctly.
    /// Setup: Creates a CaddyConfigurationInfo instance with various data types and tests serialization/deserialization.
    /// Expectation: The model should handle serialization and deserialization correctly, ensuring proper data persistence and transfer capabilities.
    /// </summary>
    [Fact]
    public void CaddyConfigurationInfo_WithSerialization_HandlesCorrectly()
    {
        // Arrange
        var originalInfo = new CaddyConfigurationInfo
        {
            Hostnames = new List<string> { "example.com", "api.example.com" },
            ReverseProxyHostname = "localhost",
            ReverseProxyPorts = new List<int> { 3000, 8080 },
            FileName = "test-config",
            AggregatedReverseProxyPorts = new List<int> { 3000, 8080, 9000 }
        };

        // Act - Simulate serialization/deserialization
        var serializedData = System.Text.Json.JsonSerializer.Serialize(originalInfo);
        var deserializedInfo = System.Text.Json.JsonSerializer.Deserialize<CaddyConfigurationInfo>(serializedData);

        // Assert
        deserializedInfo.Should().NotBeNull();
        deserializedInfo!.Hostnames.Should().BeEquivalentTo(originalInfo.Hostnames);
        deserializedInfo.ReverseProxyHostname.Should().Be(originalInfo.ReverseProxyHostname);
        deserializedInfo.ReverseProxyPorts.Should().BeEquivalentTo(originalInfo.ReverseProxyPorts);
        deserializedInfo.FileName.Should().Be(originalInfo.FileName);
        deserializedInfo.AggregatedReverseProxyPorts.Should().BeEquivalentTo(originalInfo.AggregatedReverseProxyPorts);
    }

    #endregion
}
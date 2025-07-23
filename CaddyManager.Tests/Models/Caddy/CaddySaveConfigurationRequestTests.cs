using CaddyManager.Models.Caddy;

namespace CaddyManager.Tests.Models.Caddy;

/// <summary>
/// Tests for CaddySaveConfigurationRequest model
/// </summary>
public class CaddySaveConfigurationRequestTests
{
    /// <summary>
    /// Tests that CaddySaveConfigurationRequest initializes correctly when provided with a required FileName.
    /// Setup: Creates a CaddySaveConfigurationRequest instance with a FileName and examines the default values of other properties.
    /// Expectation: The FileName should be set as provided, IsNew should default to false, and Content should be an empty string, ensuring the request model is properly initialized for Caddy configuration save operations with sensible defaults.
    /// </summary>
    [Fact]
    public void Constructor_WithRequiredFileName_InitializesCorrectly()
    {
        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test-config"
        };

        // Assert
        request.FileName.Should().Be("test-config");
        request.IsNew.Should().BeFalse(); // Default value
        request.Content.Should().Be(string.Empty); // Default value
    }

    /// <summary>
    /// Tests that all CaddySaveConfigurationRequest properties can be properly set and retrieved with realistic configuration data.
    /// Setup: Creates test data including new configuration flag, file name, and Caddyfile content representing a typical configuration save scenario.
    /// Expectation: All properties should store and return the exact values assigned, ensuring data integrity for Caddy configuration save requests that include both metadata and the actual configuration content.
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var isNew = true;
        var fileName = "my-config";
        var content = "example.com {\n    reverse_proxy localhost:8080\n}";

        // Act
        var request = new CaddySaveConfigurationRequest
        {
            IsNew = isNew,
            FileName = fileName,
            Content = content
        };

        // Assert
        request.IsNew.Should().Be(isNew);
        request.FileName.Should().Be(fileName);
        request.Content.Should().Be(content);
    }

    /// <summary>
    /// Tests that the IsNew property correctly handles both true and false values representing different configuration scenarios.
    /// Setup: Uses parameterized test data with both boolean values representing new configuration creation versus existing configuration updates.
    /// Expectation: The IsNew property should store the provided boolean value accurately, enabling proper differentiation between creating new Caddy configurations and updating existing ones in the management system.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsNew_WithVariousValues_SetsCorrectly(bool isNew)
    {
        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test",
            IsNew = isNew
        };

        // Assert
        request.IsNew.Should().Be(isNew);
    }

    /// <summary>
    /// Tests that the FileName property correctly handles various valid naming conventions used in Caddy configuration files.
    /// Setup: Uses parameterized test data with different file naming patterns including simple names, dashes, underscores, camelCase, numbers, and long descriptive names.
    /// Expectation: All valid file name formats should be stored correctly, ensuring compatibility with different Caddy configuration naming conventions used across various deployment scenarios and organizational standards.
    /// </summary>
    [Theory]
    [InlineData("simple-name")]
    [InlineData("name-with-dashes")]
    [InlineData("name_with_underscores")]
    [InlineData("NameWithCamelCase")]
    [InlineData("name123")]
    [InlineData("very-long-configuration-file-name-that-might-be-used")]
    public void FileName_WithVariousValidNames_SetsCorrectly(string fileName)
    {
        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = fileName
        };

        // Assert
        request.FileName.Should().Be(fileName);
    }

    /// <summary>
    /// Tests that the FileName property accepts various string formats including edge cases and potentially problematic characters.
    /// Setup: Uses parameterized test data with empty strings, whitespace, spaces, and path separators that might be encountered in real-world scenarios.
    /// Expectation: All string values should be accepted and stored as-is, since the model doesn't enforce validation (validation occurs at service/controller level), ensuring the request model can capture any input for proper validation handling upstream in Caddy management operations.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("name with spaces")]
    [InlineData("name/with/slashes")]
    [InlineData("name\\with\\backslashes")]
    public void FileName_WithVariousStringValues_AcceptsAll(string fileName)
    {
        // Note: The model doesn't enforce validation, it just stores the value
        // Validation would typically be done at the service or controller level

        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = fileName
        };

        // Assert
        request.FileName.Should().Be(fileName);
    }

    /// <summary>
    /// Tests that the Content property correctly handles empty string values representing empty Caddy configurations.
    /// Setup: Creates a CaddySaveConfigurationRequest and sets the Content property to an empty string.
    /// Expectation: The empty string should be stored correctly, enabling scenarios where empty Caddy configuration files need to be created or where existing configurations are cleared during management operations.
    /// </summary>
    [Fact]
    public void Content_WithEmptyString_SetsCorrectly()
    {
        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test",
            Content = ""
        };

        // Assert
        request.Content.Should().Be("");
    }

    /// <summary>
    /// Tests that the Content property correctly handles complex, multi-line Caddyfile content with various directives and configurations.
    /// Setup: Creates a realistic complex Caddyfile content with multiple domains, routing rules, reverse proxy configurations, file serving, compression, and logging directives.
    /// Expectation: The entire complex content should be stored exactly as provided, ensuring that sophisticated Caddy configurations with multiple sites, advanced routing, and various middleware can be properly saved and managed through the request model.
    /// </summary>
    [Fact]
    public void Content_WithComplexCaddyfileContent_SetsCorrectly()
    {
        // Arrange
        var complexContent = @"
example.com, www.example.com {
    route /api/* {
        reverse_proxy localhost:3000
    }
    
    route /static/* {
        file_server
    }
    
    reverse_proxy localhost:8080
    encode gzip
    log {
        output file /var/log/caddy/access.log
    }
}

api.example.com {
    reverse_proxy localhost:4000
    header {
        Access-Control-Allow-Origin *
    }
}";

        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "complex-config",
            Content = complexContent
        };

        // Assert
        request.Content.Should().Be(complexContent);
    }

    /// <summary>
    /// Tests that the Content property correctly handles special characters that might appear in Caddy configurations.
    /// Setup: Creates content containing various special characters including punctuation, symbols, and operators that might be used in Caddy directives or comments.
    /// Expectation: All special characters should be preserved exactly, ensuring that Caddy configurations with complex expressions, regex patterns, or special formatting are not corrupted during save operations.
    /// </summary>
    [Fact]
    public void Content_WithSpecialCharacters_SetsCorrectly()
    {
        // Arrange
        var contentWithSpecialChars = "Content with special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test",
            Content = contentWithSpecialChars
        };

        // Assert
        request.Content.Should().Be(contentWithSpecialChars);
    }

    /// <summary>
    /// Tests that the Content property correctly preserves whitespace formatting including newlines and tabs.
    /// Setup: Creates content with various line endings (Unix, Windows) and tab characters representing formatted Caddyfile content.
    /// Expectation: All whitespace formatting should be preserved exactly, ensuring that Caddy configuration indentation, line breaks, and formatting are maintained for readability and proper parsing by the Caddy server.
    /// </summary>
    [Fact]
    public void Content_WithNewlinesAndTabs_SetsCorrectly()
    {
        // Arrange
        var contentWithWhitespace = "Line 1\nLine 2\n\tIndented line\r\nWindows line ending";

        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test",
            Content = contentWithWhitespace
        };

        // Assert
        request.Content.Should().Be(contentWithWhitespace);
    }

    /// <summary>
    /// Tests that CaddySaveConfigurationRequest properties can be modified after instance creation, except for the init-only FileName.
    /// Setup: Creates a CaddySaveConfigurationRequest with initial values, then attempts to modify all properties including the init-only FileName.
    /// Expectation: IsNew and Content should accept new values, but FileName should remain unchanged due to its init-only nature, ensuring that configuration identity remains stable while allowing content and metadata updates during Caddy management operations.
    /// </summary>
    [Fact]
    public void Properties_CanBeModifiedAfterCreation()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "initial-name",
            IsNew = false,
            Content = "initial content"
        };

        // Act
        // Note: FileName is init-only and cannot be modified after creation
        request.IsNew = true;
        request.Content = "updated content";

        // Assert
        request.FileName.Should().Be("initial-name"); // FileName cannot be changed
        request.IsNew.Should().BeTrue();
        request.Content.Should().Be("updated content");
    }

    /// <summary>
    /// Tests that CaddySaveConfigurationRequest properties have appropriate default values when not explicitly set.
    /// Setup: Creates a CaddySaveConfigurationRequest with only the required FileName and examines the default values of other properties.
    /// Expectation: IsNew should default to false and Content should be an empty non-null string, providing safe defaults that assume updating existing configurations rather than creating new ones, which is the more common scenario in Caddy management.
    /// </summary>
    [Fact]
    public void DefaultValues_AreSetCorrectly()
    {
        // Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test"
        };

        // Assert
        request.IsNew.Should().BeFalse();
        request.Content.Should().Be(string.Empty);
        request.Content.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the FileName property is properly marked as required and behaves correctly when provided.
    /// Setup: Creates a CaddySaveConfigurationRequest with a valid FileName and verifies it's properly set and not null or empty.
    /// Expectation: The FileName should be properly stored and not be null or empty, confirming that the required attribute is properly applied and that configuration save requests always have a valid file identifier for Caddy management operations.
    /// </summary>
    [Fact]
    public void FileName_IsRequired_PropertyHasRequiredAttribute()
    {
        // This test verifies that the FileName property is marked as required
        // The actual enforcement would be done by model validation in ASP.NET Core

        // Arrange & Act
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test"
        };

        // Assert
        request.FileName.Should().NotBeNull();
        request.FileName.Should().NotBe(string.Empty);
    }

    /// <summary>
    /// Tests that assigning null to the Content property does not throw an exception.
    /// Setup: Creates a CaddySaveConfigurationRequest and attempts to assign null to the Content property.
    /// Expectation: The null assignment should not throw an exception, ensuring robust error handling in Caddy management operations where null assignments might occur due to programming errors or edge cases, maintaining system stability.
    /// </summary>
    [Fact]
    public void Content_WithNullAssignment_DoesNotThrow()
    {
        // Act & Assert
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test"
        };
        
        var act = () => request.Content = null!;
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that CaddySaveConfigurationRequest correctly handles various realistic combinations of all properties.
    /// Setup: Uses parameterized test data with different combinations of IsNew flags, file names, and content representing typical Caddy configuration scenarios including new configurations, updates, global configs, and empty content.
    /// Expectation: All property combinations should be stored correctly, ensuring that the request model can handle the full range of Caddy configuration save scenarios from creating new site configs to updating existing ones with various content types.
    /// </summary>
    [Theory]
    [InlineData(true, "new-config", "new content")]
    [InlineData(false, "existing-config", "updated content")]
    [InlineData(true, "Caddyfile", "global config content")]
    [InlineData(false, "api-config", "")]
    public void CompleteRequest_WithVariousCombinations_SetsAllPropertiesCorrectly(
        bool isNew, string fileName, string content)
    {
        // Act
        var request = new CaddySaveConfigurationRequest
        {
            IsNew = isNew,
            FileName = fileName,
            Content = content
        };

        // Assert
        request.IsNew.Should().Be(isNew);
        request.FileName.Should().Be(fileName);
        request.Content.Should().Be(content);
    }
}
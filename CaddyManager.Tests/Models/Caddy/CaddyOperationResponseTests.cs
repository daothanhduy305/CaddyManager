using CaddyManager.Models.Caddy;

namespace CaddyManager.Tests.Models.Caddy;

/// <summary>
/// Tests for CaddyOperationResponse model
/// </summary>
public class CaddyOperationResponseTests
{
    /// <summary>
    /// Tests that the CaddyOperationResponse constructor initializes all properties with appropriate default values.
    /// Setup: Creates a new CaddyOperationResponse instance using the default constructor.
    /// Expectation: Success should default to false and Message should be an empty string, ensuring that operation responses start in a safe state where success must be explicitly set, preventing false positive results in Caddy management operations.
    /// </summary>
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Act
        var response = new CaddyOperationResponse();

        // Assert
        response.Success.Should().BeFalse();
        response.Message.Should().Be(string.Empty);
    }

    /// <summary>
    /// Tests that all CaddyOperationResponse properties can be properly set and retrieved with realistic operation data.
    /// Setup: Creates test data with success status and a descriptive message representing a typical Caddy operation completion scenario.
    /// Expectation: Both properties should store and return the exact values assigned, ensuring reliable communication of operation results between Caddy management services and client applications.
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var success = true;
        var message = "Operation completed successfully";

        // Act
        var response = new CaddyOperationResponse
        {
            Success = success,
            Message = message
        };

        // Assert
        response.Success.Should().Be(success);
        response.Message.Should().Be(message);
    }

    /// <summary>
    /// Tests that Success and Message properties handle various combinations of values representing different operation outcomes.
    /// Setup: Uses parameterized test data with different success states and message content, including empty messages, representing the range of possible Caddy operation results.
    /// Expectation: All combinations should be stored correctly, ensuring that the base response model can accurately represent any type of Caddy operation outcome, whether successful or failed, with or without detailed messages.
    /// </summary>
    [Theory]
    [InlineData(true, "Success message")]
    [InlineData(false, "Error message")]
    [InlineData(true, "")]
    [InlineData(false, "")]
    public void Properties_WithVariousValues_SetCorrectly(bool success, string message)
    {
        // Act
        var response = new CaddyOperationResponse
        {
            Success = success,
            Message = message
        };

        // Assert
        response.Success.Should().Be(success);
        response.Message.Should().Be(message);
    }

    /// <summary>
    /// Tests that assigning null to the Message property handles the null value appropriately.
    /// Setup: Creates a CaddyOperationResponse instance and assigns null to the Message property.
    /// Expectation: The Message property should not be null after assignment, ensuring robust null handling in Caddy operation responses and preventing null reference exceptions in downstream message processing operations.
    /// </summary>
    [Fact]
    public void Message_WithNullValue_SetsToEmptyString()
    {
        // Act
        var response = new CaddyOperationResponse
        {
            Message = null!
        };

        // Assert
        response.Message.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that the Success property has a default value of false when not explicitly set.
    /// Setup: Creates a new CaddyOperationResponse instance without setting the Success property.
    /// Expectation: Success should be false by default, implementing a fail-safe approach where operations are considered unsuccessful until explicitly marked as successful, ensuring conservative error handling in Caddy management operations.
    /// </summary>
    [Fact]
    public void Success_DefaultValue_IsFalse()
    {
        // Act
        var response = new CaddyOperationResponse();

        // Assert
        response.Success.Should().BeFalse();
    }

    /// <summary>
    /// Tests that the Message property has a default value of empty string when not explicitly set.
    /// Setup: Creates a new CaddyOperationResponse instance without setting the Message property.
    /// Expectation: Message should be an empty but non-null string by default, ensuring that message handling code doesn't encounter null references and providing a consistent baseline for Caddy operation response messaging.
    /// </summary>
    [Fact]
    public void Message_DefaultValue_IsEmptyString()
    {
        // Act
        var response = new CaddyOperationResponse();

        // Assert
        response.Message.Should().Be(string.Empty);
        response.Message.Should().NotBeNull();
    }

    /// <summary>
    /// Tests that CaddyOperationResponse properties can be modified after the instance is created.
    /// Setup: Creates a CaddyOperationResponse with initial values, then updates both properties with new values representing a change in operation status.
    /// Expectation: Both properties should accept the new values and completely replace the old ones, enabling dynamic updates to operation results as Caddy management operations progress or are re-evaluated.
    /// </summary>
    [Fact]
    public void Properties_CanBeModifiedAfterCreation()
    {
        // Arrange
        var response = new CaddyOperationResponse
        {
            Success = false,
            Message = "Initial message"
        };

        // Act
        response.Success = true;
        response.Message = "Updated message";

        // Assert
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated message");
    }

    /// <summary>
    /// Tests that the Message property correctly handles various string formats and content types.
    /// Setup: Uses parameterized test data with different message formats including empty strings, whitespace, short text, long descriptions, special characters, and formatted text with newlines and tabs.
    /// Expectation: All message formats should be stored exactly as provided, ensuring that Caddy operation responses can contain any type of diagnostic information, error details, or success messages without data corruption or formatting issues.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Short")]
    [InlineData("This is a very long message that contains multiple words and should be handled correctly by the model")]
    [InlineData("Message with special characters: !@#$%^&*()_+-=[]{}|;':\",./<>?")]
    [InlineData("Message with\nnewlines\nand\ttabs")]
    public void Message_WithVariousStringValues_SetsCorrectly(string message)
    {
        // Act
        var response = new CaddyOperationResponse
        {
            Message = message
        };

        // Assert
        response.Message.Should().Be(message);
    }
}
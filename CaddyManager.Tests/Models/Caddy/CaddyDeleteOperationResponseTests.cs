using CaddyManager.Contracts.Models.Caddy;

namespace CaddyManager.Tests.Models.Caddy;

/// <summary>
/// Tests for CaddyDeleteOperationResponse model
/// </summary>
public class CaddyDeleteOperationResponseTests
{
    /// <summary>
    /// Tests that the CaddyDeleteOperationResponse constructor initializes all properties with appropriate default values.
    /// Setup: Creates a new CaddyDeleteOperationResponse instance using the default constructor.
    /// Expectation: Inherited properties should have base class defaults (Success=false, Message=empty), and DeletedConfigurations should be initialized as an empty but non-null collection, ensuring the response model is ready for immediate use in Caddy configuration deletion operations.
    /// </summary>
    [Fact]
    public void Constructor_InitializesWithDefaultValues()
    {
        // Act
        var response = new CaddyDeleteOperationResponse();

        // Assert
        response.Success.Should().BeFalse(); // Inherited from base class
        response.Message.Should().Be(string.Empty); // Inherited from base class
        response.DeletedConfigurations.Should().NotBeNull();
        response.DeletedConfigurations.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that CaddyDeleteOperationResponse properly inherits from CaddyOperationResponse base class.
    /// Setup: Creates a CaddyDeleteOperationResponse instance and checks its type hierarchy.
    /// Expectation: The instance should be assignable to CaddyOperationResponse, ensuring proper inheritance structure for consistent operation response handling across different Caddy management operations while maintaining specialized delete-specific functionality.
    /// </summary>
    [Fact]
    public void InheritsFromCaddyOperationResponse()
    {
        // Act
        var response = new CaddyDeleteOperationResponse();

        // Assert
        response.Should().BeAssignableTo<CaddyOperationResponse>();
    }

    /// <summary>
    /// Tests that all CaddyDeleteOperationResponse properties can be properly set and retrieved with realistic data.
    /// Setup: Creates test data including success status, descriptive message, and a list of deleted configuration names representing a typical Caddy configuration deletion scenario.
    /// Expectation: All properties should store and return the exact values assigned, ensuring data integrity for tracking which configurations were successfully deleted during Caddy management operations.
    /// </summary>
    [Fact]
    public void Properties_CanBeSetAndRetrieved()
    {
        // Arrange
        var success = true;
        var message = "Configurations deleted successfully";
        var deletedConfigurations = new List<string> { "config1", "config2", "config3" };

        // Act
        var response = new CaddyDeleteOperationResponse
        {
            Success = success,
            Message = message,
            DeletedConfigurations = deletedConfigurations
        };

        // Assert
        response.Success.Should().Be(success);
        response.Message.Should().Be(message);
        response.DeletedConfigurations.Should().BeEquivalentTo(deletedConfigurations);
    }

    /// <summary>
    /// Tests that the DeletedConfigurations collection can be modified after CaddyDeleteOperationResponse creation.
    /// Setup: Creates a CaddyDeleteOperationResponse instance and adds configuration names to the DeletedConfigurations collection.
    /// Expectation: The collection should accept new entries and maintain them correctly, enabling dynamic tracking of deleted configurations during batch deletion operations in Caddy management workflows.
    /// </summary>
    [Fact]
    public void DeletedConfigurations_CanBeModified()
    {
        // Arrange
        var response = new CaddyDeleteOperationResponse();

        // Act
        response.DeletedConfigurations.Add("config1");
        response.DeletedConfigurations.Add("config2");

        // Assert
        response.DeletedConfigurations.Should().HaveCount(2);
        response.DeletedConfigurations.Should().Contain("config1");
        response.DeletedConfigurations.Should().Contain("config2");
    }

    /// <summary>
    /// Tests that assigning an empty list to DeletedConfigurations maintains the empty state correctly.
    /// Setup: Creates a CaddyDeleteOperationResponse and explicitly assigns an empty list to DeletedConfigurations.
    /// Expectation: The collection should remain empty but non-null, ensuring proper handling of scenarios where no configurations were deleted during Caddy management operations, which is important for accurate operation reporting.
    /// </summary>
    [Fact]
    public void DeletedConfigurations_WithEmptyList_RemainsEmpty()
    {
        // Act
        var response = new CaddyDeleteOperationResponse
        {
            DeletedConfigurations = new List<string>()
        };

        // Assert
        response.DeletedConfigurations.Should().NotBeNull();
        response.DeletedConfigurations.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that assigning null to DeletedConfigurations does not throw an exception.
    /// Setup: Creates a CaddyDeleteOperationResponse instance and attempts to assign null to the DeletedConfigurations property.
    /// Expectation: The assignment should not throw an exception, ensuring robust error handling in Caddy management operations where null assignments might occur due to programming errors or edge cases, maintaining system stability.
    /// </summary>
    [Fact]
    public void DeletedConfigurations_WithNullAssignment_DoesNotThrow()
    {
        // Act & Assert
        var response = new CaddyDeleteOperationResponse();
        var act = () => response.DeletedConfigurations = null!;
        act.Should().NotThrow();
    }

    /// <summary>
    /// Tests that Success and Message properties can be set with various combinations representing different deletion outcomes.
    /// Setup: Uses parameterized test data with different success states and corresponding messages that represent typical Caddy configuration deletion scenarios.
    /// Expectation: Both properties should store the provided values correctly, ensuring accurate reporting of deletion operation results in Caddy management systems, whether operations succeed completely or encounter failures.
    /// </summary>
    [Theory]
    [InlineData(true, "All configurations deleted successfully")]
    [InlineData(false, "Failed to delete some configurations")]
    public void SuccessAndMessage_WithVariousValues_SetCorrectly(bool success, string message)
    {
        // Act
        var response = new CaddyDeleteOperationResponse
        {
            Success = success,
            Message = message
        };

        // Assert
        response.Success.Should().Be(success);
        response.Message.Should().Be(message);
    }

    /// <summary>
    /// Tests that the DeletedConfigurations collection allows duplicate configuration names.
    /// Setup: Creates a CaddyDeleteOperationResponse and adds duplicate configuration names to the DeletedConfigurations collection.
    /// Expectation: The collection should accept and store duplicate entries, which might occur in edge cases where the same configuration is processed multiple times during complex Caddy management operations, ensuring complete audit trails.
    /// </summary>
    [Fact]
    public void DeletedConfigurations_WithDuplicateEntries_AllowsDuplicates()
    {
        // Arrange
        var response = new CaddyDeleteOperationResponse();

        // Act
        response.DeletedConfigurations.Add("config1");
        response.DeletedConfigurations.Add("config1");
        response.DeletedConfigurations.Add("config2");

        // Assert
        response.DeletedConfigurations.Should().HaveCount(3);
        response.DeletedConfigurations.Should().Contain("config1");
        response.DeletedConfigurations.Should().Contain("config2");
        response.DeletedConfigurations.Count(x => x == "config1").Should().Be(2);
    }

    /// <summary>
    /// Tests that the DeletedConfigurations collection properly handles various string formats including edge cases.
    /// Setup: Creates a collection with different string types including empty strings, whitespace, normal names, special characters, and very long names representing diverse Caddy configuration naming patterns.
    /// Expectation: All string values should be stored correctly regardless of format, ensuring robust handling of different configuration naming conventions used in various Caddy deployment scenarios and preventing data loss during deletion tracking.
    /// </summary>
    [Fact]
    public void DeletedConfigurations_WithVariousStringValues_HandlesCorrectly()
    {
        // Arrange
        var configurations = new List<string>
        {
            "",
            "   ",
            "normal-config",
            "config-with-special-chars!@#$%",
            "very-long-configuration-name-that-might-be-used-in-some-scenarios"
        };

        // Act
        var response = new CaddyDeleteOperationResponse
        {
            DeletedConfigurations = configurations
        };

        // Assert
        response.DeletedConfigurations.Should().BeEquivalentTo(configurations);
        response.DeletedConfigurations.Should().HaveCount(5);
    }

    /// <summary>
    /// Tests that all CaddyDeleteOperationResponse properties can be modified after the instance is created.
    /// Setup: Creates a CaddyDeleteOperationResponse with initial values, then updates all properties with new values representing a change in deletion operation status.
    /// Expectation: All properties should accept the new values and the old values should be completely replaced, enabling dynamic updates to deletion operation results as Caddy management operations progress or are re-evaluated.
    /// </summary>
    [Fact]
    public void Properties_CanBeModifiedAfterCreation()
    {
        // Arrange
        var response = new CaddyDeleteOperationResponse
        {
            Success = false,
            Message = "Initial message",
            DeletedConfigurations = new List<string> { "initial-config" }
        };

        // Act
        response.Success = true;
        response.Message = "Updated message";
        response.DeletedConfigurations = new List<string> { "updated-config1", "updated-config2" };

        // Assert
        response.Success.Should().BeTrue();
        response.Message.Should().Be("Updated message");
        response.DeletedConfigurations.Should().HaveCount(2);
        response.DeletedConfigurations.Should().Contain("updated-config1");
        response.DeletedConfigurations.Should().Contain("updated-config2");
        response.DeletedConfigurations.Should().NotContain("initial-config");
    }

    /// <summary>
    /// Tests that the DeletedConfigurations property is properly initialized as an empty List&lt;string&gt; by default.
    /// Setup: Creates a new CaddyDeleteOperationResponse instance and examines the DeletedConfigurations property.
    /// Expectation: The property should be a non-null, empty List&lt;string&gt; instance, ensuring that deletion tracking is immediately available without additional initialization and preventing null reference exceptions in Caddy configuration management operations.
    /// </summary>
    [Fact]
    public void DeletedConfigurations_DefaultInitialization_IsEmptyList()
    {
        // Act
        var response = new CaddyDeleteOperationResponse();

        // Assert
        response.DeletedConfigurations.Should().NotBeNull();
        response.DeletedConfigurations.Should().BeOfType<List<string>>();
        response.DeletedConfigurations.Should().BeEmpty();
    }
}
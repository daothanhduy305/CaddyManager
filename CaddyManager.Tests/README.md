# CaddyManager Tests

This project contains comprehensive unit tests for the CaddyManager application.

## Test Structure

The tests are organized to mirror the main project structure:

### Services Tests
- **CaddyConfigurationParsingService**: Tests for parsing Caddyfile content and extracting hostnames, reverse proxy targets, and ports
- **CaddyService**: Tests for managing Caddy configuration files (CRUD operations)
- **ConfigurationsService**: Tests for configuration management and dependency injection
- **DockerService**: Tests for Docker container management functionality

### Models Tests
- **CaddyConfigurationInfo**: Tests for configuration information model
- **CaddyOperationResponse**: Tests for operation response model
- **CaddyDeleteOperationResponse**: Tests for delete operation response model
- **CaddySaveConfigurationRequest**: Tests for save configuration request model

### Configuration Tests
- **CaddyServiceConfigurations**: Tests for Caddy service configuration class
- **DockerServiceConfiguration**: Tests for Docker service configuration class

### Test Utilities
- **TestHelper**: Common utilities for creating test data, temporary files, and configurations

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run tests with coverage
```bash
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings
```

### Run specific test class
```bash
dotnet test --filter "ClassName=CaddyServiceTests"
```

### Run tests with verbose output
```bash
dotnet test --verbosity normal
```

## Test Coverage

The project is configured to generate code coverage reports in multiple formats:
- OpenCover
- Cobertura
- JSON
- LCOV
- TeamCity
- HTML

Coverage reports exclude:
- Test projects (`[*.Tests]*`)
- Program entry points (`[*]*.Program`)
- Migration files (`[*]*Migrations*`)
- Generated code (marked with appropriate attributes)

## Test Frameworks and Libraries

- **xUnit**: Primary testing framework
- **FluentAssertions**: For readable assertions
- **Moq**: For mocking dependencies
- **Coverlet**: For code coverage analysis

## Test Patterns

The tests follow these patterns:

### Arrange-Act-Assert (AAA)
All tests use the AAA pattern for clarity:
```csharp
[Fact]
public void Method_Condition_ExpectedResult()
{
    // Arrange
    var input = "test input";
    
    // Act
    var result = service.Method(input);
    
    // Assert
    result.Should().Be("expected output");
}
```

### Theory Tests
For testing multiple scenarios:
```csharp
[Theory]
[InlineData("input1", "output1")]
[InlineData("input2", "output2")]
public void Method_WithVariousInputs_ReturnsExpectedOutputs(string input, string expected)
{
    // Test implementation
}
```

### Mocking
Dependencies are mocked using Moq:
```csharp
var mockService = new Mock<IService>();
mockService.Setup(x => x.Method(It.IsAny<string>())).Returns("mocked result");
```

### Test Data
Common test data is provided through the `TestHelper` class:
- Sample Caddyfile configurations
- Temporary file and directory creation
- Configuration builders

## Best Practices

1. **Test Naming**: Use descriptive names that indicate the method being tested, the condition, and the expected result
2. **Single Responsibility**: Each test should verify one specific behavior
3. **Independence**: Tests should not depend on each other and should be able to run in any order
4. **Cleanup**: Use `IDisposable` or cleanup methods to remove temporary resources
5. **Readable Assertions**: Use FluentAssertions for more readable test assertions
6. **Mock Verification**: Verify that mocked methods are called as expected when relevant

## Continuous Integration

These tests are designed to run in CI/CD environments and include:
- Fast execution times
- No external dependencies (except for Docker integration tests which are skipped)
- Comprehensive coverage of business logic
- Clear failure messages for debugging
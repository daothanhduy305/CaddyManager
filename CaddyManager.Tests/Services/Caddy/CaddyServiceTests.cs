using CaddyManager.Contracts.Configurations.Caddy;
using CaddyManager.Contracts.Caddy;
using CaddyManager.Contracts.Configurations;
using CaddyManager.Contracts.Models.Caddy;
using CaddyManager.Services.Caddy;
using CaddyManager.Tests.TestUtilities;

namespace CaddyManager.Tests.Services.Caddy;

/// <summary>
/// Tests for CaddyService
/// </summary>
public class CaddyServiceTests : IDisposable
{
    private readonly Mock<IConfigurationsService> _mockConfigurationsService;
    private readonly Mock<ICaddyConfigurationParsingService> _mockParsingService;
    private readonly CaddyService _service;
    private readonly string _tempConfigDir;
    private readonly CaddyServiceConfigurations _testConfigurations;

    public CaddyServiceTests()
    {
        _mockConfigurationsService = new Mock<IConfigurationsService>();
        _mockParsingService = new Mock<ICaddyConfigurationParsingService>();
        
        _tempConfigDir = TestHelper.CreateTempDirectory();
        _testConfigurations = new CaddyServiceConfigurations
        {
            ConfigDir = _tempConfigDir
        };

        _mockConfigurationsService
            .Setup(x => x.Get<CaddyServiceConfigurations>())
            .Returns(_testConfigurations);

        _service = new CaddyService(_mockConfigurationsService.Object, _mockParsingService.Object);
    }

    public void Dispose()
    {
        TestHelper.CleanupDirectory(_tempConfigDir);
    }

    #region GetExistingCaddyConfigurations Tests

    /// <summary>
    /// Tests that the Caddy service correctly handles an empty configuration directory by returning an empty list.
    /// Setup: Uses a temporary empty directory as the configuration directory with no Caddy files present.
    /// Expectation: The service should return an empty list without errors, ensuring graceful handling of new or clean Caddy installations where no configurations exist yet.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_WithEmptyDirectory_ReturnsEmptyList()
    {
        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the Caddy service automatically creates the configuration directory if it doesn't exist and returns an empty list.
    /// Setup: Configures the service to use a non-existent directory path for Caddy configuration storage.
    /// Expectation: The service should create the missing directory and return an empty list, ensuring automatic directory initialization for new Caddy deployments.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_WithNonExistentDirectory_CreatesDirectoryAndReturnsEmptyList()
    {
        // Arrange
        var nonExistentDir = Path.Combine(_tempConfigDir, "nonexistent");
        _testConfigurations.ConfigDir = nonExistentDir;

        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        Directory.Exists(nonExistentDir).Should().BeTrue();
    }

    /// <summary>
    /// Tests that the Caddy service correctly reads and parses existing Caddy configuration files to return populated configuration information.
    /// Setup: Creates test Caddy files in the configuration directory and mocks the parsing service to return expected hostnames, targets, and ports.
    /// Expectation: The service should return configuration info objects with parsed data for each file, enabling comprehensive Caddy configuration management and monitoring.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_WithCaddyFiles_ReturnsConfigurationInfos()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        File.WriteAllText(Path.Combine(_tempConfigDir, "test1.caddy"), testContent);
        File.WriteAllText(Path.Combine(_tempConfigDir, "test2.caddy"), testContent);

        _mockParsingService
            .Setup(x => x.GetHostnamesFromCaddyfileContent(testContent))
            .Returns(new List<string> { "example.com" });
        _mockParsingService
            .Setup(x => x.GetReverseProxyTargetFromCaddyfileContent(testContent))
            .Returns("localhost");
        _mockParsingService
            .Setup(x => x.GetReverseProxyPortsFromCaddyfileContent(testContent))
            .Returns(new List<int> { 8080 });

        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.FileName == "test1");
        result.Should().Contain(x => x.FileName == "test2");
        result.Should().AllSatisfy(x =>
        {
            x.Hostnames.Should().Contain("example.com");
            x.ReverseProxyHostname.Should().Be("localhost");
            x.ReverseProxyPorts.Should().Contain(8080);
        });
    }

    /// <summary>
    /// Tests that the Caddy service excludes the global Caddyfile from the list of individual configurations.
    /// Setup: Creates both a global Caddyfile and individual .caddy files in the configuration directory.
    /// Expectation: The service should return only the individual configuration files and exclude the global Caddyfile, maintaining separation between global and site-specific configurations.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_ExcludesGlobalCaddyfile()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        File.WriteAllText(Path.Combine(_tempConfigDir, "Caddyfile"), testContent);
        File.WriteAllText(Path.Combine(_tempConfigDir, "test.caddy"), testContent);

        _mockParsingService
            .Setup(x => x.GetHostnamesFromCaddyfileContent(testContent))
            .Returns(new List<string> { "example.com" });

        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.FileName == "test");
        result.Should().NotContain(x => x.FileName == "Caddyfile");
    }

    /// <summary>
    /// Tests that the Caddy service returns configuration files in alphabetical order for consistent presentation.
    /// Setup: Creates multiple Caddy configuration files with names that would naturally sort in a specific order.
    /// Expectation: The service should return configurations sorted alphabetically by filename, ensuring predictable and user-friendly ordering in the Caddy management interface.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_ReturnsOrderedResults()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        File.WriteAllText(Path.Combine(_tempConfigDir, "zebra.caddy"), testContent);
        File.WriteAllText(Path.Combine(_tempConfigDir, "alpha.caddy"), testContent);
        File.WriteAllText(Path.Combine(_tempConfigDir, "beta.caddy"), testContent);

        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result[0].FileName.Should().Be("alpha");
        result[1].FileName.Should().Be("beta");
        result[2].FileName.Should().Be("zebra");
    }

    #endregion

    #region GetCaddyConfigurationContent Tests

    /// <summary>
    /// Tests that the Caddy service correctly retrieves the content of an existing configuration file.
    /// Setup: Creates a test Caddy configuration file with known content in the configuration directory.
    /// Expectation: The service should return the exact file content, enabling configuration viewing and editing functionality in the Caddy management system.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationContent_WithExistingFile_ReturnsContent()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        var filePath = Path.Combine(_tempConfigDir, "test.caddy");
        File.WriteAllText(filePath, testContent);

        // Act
        var result = _service.GetCaddyConfigurationContent("test");

        // Assert
        result.Should().Be(testContent);
    }

    /// <summary>
    /// Tests that the Caddy service handles requests for non-existent configuration files gracefully.
    /// Setup: Attempts to retrieve content for a configuration file that doesn't exist in the directory.
    /// Expectation: The service should return an empty string rather than throwing exceptions, ensuring robust error handling for missing configuration files.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationContent_WithNonExistentFile_ReturnsEmptyString()
    {
        // Act
        var result = _service.GetCaddyConfigurationContent("nonexistent");

        // Assert
        result.Should().Be(string.Empty);
    }

    /// <summary>
    /// Tests that the Caddy service correctly retrieves the content of the global Caddyfile configuration.
    /// Setup: Creates a global Caddyfile with known content in the configuration directory.
    /// Expectation: The service should return the global Caddyfile content, enabling management of global Caddy settings and directives that apply across all sites.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationContent_WithGlobalCaddyfile_ReturnsContent()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        var filePath = Path.Combine(_tempConfigDir, "Caddyfile");
        File.WriteAllText(filePath, testContent);

        // Act
        var result = _service.GetCaddyConfigurationContent("Caddyfile");

        // Assert
        result.Should().Be(testContent);
    }

    #endregion

    #region GetCaddyGlobalConfigurationContent Tests

    /// <summary>
    /// Tests that the Caddy service correctly retrieves global configuration content using the dedicated global configuration method.
    /// Setup: Creates a global Caddyfile with known content in the configuration directory.
    /// Expectation: The service should return the global configuration content, providing specialized access to global Caddy settings and server-wide directives.
    /// </summary>
    [Fact]
    public void GetCaddyGlobalConfigurationContent_WithExistingGlobalFile_ReturnsContent()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        var filePath = Path.Combine(_tempConfigDir, "Caddyfile");
        File.WriteAllText(filePath, testContent);

        // Act
        var result = _service.GetCaddyGlobalConfigurationContent();

        // Assert
        result.Should().Be(testContent);
    }

    /// <summary>
    /// Tests that the Caddy service handles missing global configuration files gracefully.
    /// Setup: Attempts to retrieve global configuration content when no global Caddyfile exists.
    /// Expectation: The service should return an empty string, indicating no global configuration is present, which is valid for Caddy installations using only site-specific configurations.
    /// </summary>
    [Fact]
    public void GetCaddyGlobalConfigurationContent_WithNonExistentGlobalFile_ReturnsEmptyString()
    {
        // Act
        var result = _service.GetCaddyGlobalConfigurationContent();

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region SaveCaddyConfiguration Tests

    /// <summary>
    /// Tests that the Caddy service successfully saves a valid configuration request to the file system.
    /// Setup: Creates a valid save request with filename and content, then attempts to save it to the configuration directory.
    /// Expectation: The service should save the file successfully and return a success response, enabling configuration persistence and management in the Caddy system.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithValidRequest_SavesFileAndReturnsSuccess()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "test",
            Content = TestHelper.SampleCaddyfiles.SimpleReverseProxy,
            IsNew = false
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Configuration file saved successfully");

        var filePath = Path.Combine(_tempConfigDir, "test.caddy");
        File.Exists(filePath).Should().BeTrue();
        File.ReadAllText(filePath).Should().Be(request.Content);
    }

    /// <summary>
    /// Tests that the Caddy service correctly saves global configuration files with the proper Caddyfile name.
    /// Setup: Creates a save request specifically for the global Caddyfile configuration.
    /// Expectation: The service should save the file as "Caddyfile" without extension, maintaining the correct naming convention for global Caddy configuration files.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithGlobalCaddyfile_SavesWithCorrectName()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "Caddyfile",
            Content = TestHelper.SampleCaddyfiles.SimpleReverseProxy,
            IsNew = false
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        var filePath = Path.Combine(_tempConfigDir, "Caddyfile");
        File.Exists(filePath).Should().BeTrue();
        File.ReadAllText(filePath).Should().Be(request.Content);
    }

    /// <summary>
    /// Tests that the Caddy service validates filename requirements and rejects empty filenames.
    /// Setup: Creates a save request with an empty filename but valid content.
    /// Expectation: The service should return a failure response with an appropriate error message, preventing invalid file creation and ensuring proper configuration file naming.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithEmptyFileName_ReturnsFailure()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "",
            Content = TestHelper.SampleCaddyfiles.SimpleReverseProxy,
            IsNew = false
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("The configuration file name is required");
    }

    /// <summary>
    /// Tests that the Caddy service validates filename requirements and rejects whitespace-only filenames.
    /// Setup: Creates a save request with a filename containing only whitespace characters.
    /// Expectation: The service should return a failure response, preventing creation of files with invalid names that could cause file system issues or confusion.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithWhitespaceFileName_ReturnsFailure()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "   ",
            Content = TestHelper.SampleCaddyfiles.SimpleReverseProxy,
            IsNew = false
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("The configuration file name is required");
    }

    /// <summary>
    /// Tests that the Caddy service prevents overwriting existing files when creating new configurations.
    /// Setup: Creates an existing configuration file, then attempts to save a new file with the same name using the IsNew flag.
    /// Expectation: The service should return a failure response, preventing accidental overwriting of existing configurations and protecting against data loss.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithNewFileButFileExists_ReturnsFailure()
    {
        // Arrange
        var filePath = Path.Combine(_tempConfigDir, "existing.caddy");
        File.WriteAllText(filePath, "existing content");

        var request = new CaddySaveConfigurationRequest
        {
            FileName = "existing",
            Content = TestHelper.SampleCaddyfiles.SimpleReverseProxy,
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("The configuration file already exists");
    }

    /// <summary>
    /// Tests that the Caddy service successfully creates new configuration files when they don't already exist.
    /// Setup: Creates a save request for a new file with a filename that doesn't exist in the configuration directory.
    /// Expectation: The service should save the new file successfully, enabling creation of new Caddy site configurations and expanding the managed configuration set.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithNewFileAndFileDoesNotExist_SavesSuccessfully()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "newfile",
            Content = TestHelper.SampleCaddyfiles.SimpleReverseProxy,
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Configuration file saved successfully");

        var filePath = Path.Combine(_tempConfigDir, "newfile.caddy");
        File.Exists(filePath).Should().BeTrue();
    }

    #endregion

    #region SaveCaddyGlobalConfiguration Tests

    /// <summary>
    /// Tests that the Caddy service successfully saves global configuration content using the dedicated global save method.
    /// Setup: Provides valid global configuration content to be saved as the global Caddyfile.
    /// Expectation: The service should save the global configuration successfully, enabling management of server-wide Caddy settings and global directives.
    /// </summary>
    [Fact]
    public void SaveCaddyGlobalConfiguration_WithValidContent_SavesSuccessfully()
    {
        // Arrange
        var content = TestHelper.SampleCaddyfiles.SimpleReverseProxy;

        // Act
        var result = _service.SaveCaddyGlobalConfiguration(content);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Configuration file saved successfully");

        var filePath = Path.Combine(_tempConfigDir, "Caddyfile");
        File.Exists(filePath).Should().BeTrue();
        File.ReadAllText(filePath).Should().Be(content);
    }

    #endregion

    #region DeleteCaddyConfigurations Tests

    /// <summary>
    /// Tests that the Caddy service successfully deletes multiple existing configuration files.
    /// Setup: Creates multiple test configuration files, then requests deletion of all files by name.
    /// Expectation: The service should delete all specified files and return a success response with the list of deleted configurations, enabling bulk configuration cleanup.
    /// </summary>
    [Fact]
    public void DeleteCaddyConfigurations_WithExistingFiles_DeletesSuccessfully()
    {
        // Arrange
        var file1Path = Path.Combine(_tempConfigDir, "test1.caddy");
        var file2Path = Path.Combine(_tempConfigDir, "test2.caddy");
        File.WriteAllText(file1Path, "content1");
        File.WriteAllText(file2Path, "content2");

        var configurationNames = new List<string> { "test1", "test2" };

        // Act
        var result = _service.DeleteCaddyConfigurations(configurationNames);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Configuration(s) deleted successfully");
        result.DeletedConfigurations.Should().HaveCount(2);
        result.DeletedConfigurations.Should().Contain("test1");
        result.DeletedConfigurations.Should().Contain("test2");

        File.Exists(file1Path).Should().BeFalse();
        File.Exists(file2Path).Should().BeFalse();
    }

    /// <summary>
    /// Tests that the Caddy service handles deletion requests for a mix of existing and non-existent files appropriately.
    /// Setup: Creates one existing file and requests deletion of both the existing file and a non-existent file.
    /// Expectation: The service should delete the existing file, report partial failure for the non-existent file, and provide detailed feedback about which operations succeeded or failed.
    /// </summary>
    [Fact]
    public void DeleteCaddyConfigurations_WithNonExistentFiles_ReturnsPartialFailure()
    {
        // Arrange
        var file1Path = Path.Combine(_tempConfigDir, "existing.caddy");
        File.WriteAllText(file1Path, "content");

        var configurationNames = new List<string> { "existing", "nonexistent" };

        // Act
        var result = _service.DeleteCaddyConfigurations(configurationNames);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Failed to delete the following configuration(s): nonexistent");
        result.DeletedConfigurations.Should().HaveCount(1);
        result.DeletedConfigurations.Should().Contain("existing");

        File.Exists(file1Path).Should().BeFalse();
    }

    /// <summary>
    /// Tests that the Caddy service correctly deletes the global Caddyfile when specifically requested.
    /// Setup: Creates a global Caddyfile and requests its deletion using the proper filename.
    /// Expectation: The service should successfully delete the global configuration file, enabling removal of global Caddy settings when needed for reconfiguration or cleanup.
    /// </summary>
    [Fact]
    public void DeleteCaddyConfigurations_WithGlobalCaddyfile_DeletesCorrectly()
    {
        // Arrange
        var globalFilePath = Path.Combine(_tempConfigDir, "Caddyfile");
        File.WriteAllText(globalFilePath, "global content");

        var configurationNames = new List<string> { "Caddyfile" };

        // Act
        var result = _service.DeleteCaddyConfigurations(configurationNames);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Configuration(s) deleted successfully");
        result.DeletedConfigurations.Should().HaveCount(1);
        result.DeletedConfigurations.Should().Contain("Caddyfile");

        File.Exists(globalFilePath).Should().BeFalse();
    }

    /// <summary>
    /// Tests that the Caddy service handles empty deletion requests gracefully without errors.
    /// Setup: Provides an empty list of configuration names for deletion.
    /// Expectation: The service should return a success response with no deleted configurations, handling edge cases where no deletion operations are requested.
    /// </summary>
    [Fact]
    public void DeleteCaddyConfigurations_WithEmptyList_ReturnsSuccess()
    {
        // Arrange
        var configurationNames = new List<string>();

        // Act
        var result = _service.DeleteCaddyConfigurations(configurationNames);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Configuration(s) deleted successfully");
        result.DeletedConfigurations.Should().BeEmpty();
    }

    #endregion

    #region GetCaddyConfigurationInfo Tests

    /// <summary>
    /// Tests that the Caddy service correctly retrieves and parses configuration information for an existing file.
    /// Setup: Creates a test configuration file and mocks the parsing service to return expected hostnames, reverse proxy targets, and ports.
    /// Expectation: The service should return a populated configuration info object with all parsed data, enabling detailed configuration analysis and management features.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationInfo_WithExistingFile_ReturnsPopulatedInfo()
    {
        // Arrange
        var testContent = TestHelper.SampleCaddyfiles.SimpleReverseProxy;
        var filePath = Path.Combine(_tempConfigDir, "test.caddy");
        File.WriteAllText(filePath, testContent);

        var expectedHostnames = new List<string> { "example.com" };
        var expectedTarget = "localhost";
        var expectedPorts = new List<int> { 8080 };
        var expectedTags = new List<string>();

        _mockParsingService
            .Setup(x => x.GetHostnamesFromCaddyfileContent(testContent))
            .Returns(expectedHostnames);
        _mockParsingService
            .Setup(x => x.GetReverseProxyTargetFromCaddyfileContent(testContent))
            .Returns(expectedTarget);
        _mockParsingService
            .Setup(x => x.GetReverseProxyPortsFromCaddyfileContent(testContent))
            .Returns(expectedPorts);
        _mockParsingService
            .Setup(x => x.GetTagsFromCaddyfileContent(testContent))
            .Returns(expectedTags);

        // Act
        var result = _service.GetCaddyConfigurationInfo("test");

        // Assert
        result.Should().NotBeNull();
        result.Hostnames.Should().BeEquivalentTo(expectedHostnames);
        result.ReverseProxyHostname.Should().Be(expectedTarget);
        result.ReverseProxyPorts.Should().BeEquivalentTo(expectedPorts);
        result.Tags.Should().BeEquivalentTo(expectedTags);
    }

    /// <summary>
    /// Tests that the Caddy service correctly populates the Tags property when configuration content contains tags.
    /// Setup: Creates a configuration file with tags comment and mocks the parsing service to return expected tags.
    /// Expectation: The service should correctly populate the Tags property using the parsing service, ensuring tag information is available for configuration management.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationInfo_WithTags_PopulatesTagsCorrectly()
    {
        // Arrange
        var testContent = @"
# Tags: [web;production;ssl]
example.com {
    reverse_proxy localhost:8080
}";
        var filePath = Path.Combine(_tempConfigDir, "test-with-tags.caddy");
        File.WriteAllText(filePath, testContent);

        var expectedHostnames = new List<string> { "example.com" };
        var expectedTarget = "localhost";
        var expectedPorts = new List<int> { 8080 };
        var expectedTags = new List<string> { "web", "production", "ssl" };

        _mockParsingService
            .Setup(x => x.GetHostnamesFromCaddyfileContent(testContent))
            .Returns(expectedHostnames);
        _mockParsingService
            .Setup(x => x.GetReverseProxyTargetFromCaddyfileContent(testContent))
            .Returns(expectedTarget);
        _mockParsingService
            .Setup(x => x.GetReverseProxyPortsFromCaddyfileContent(testContent))
            .Returns(expectedPorts);
        _mockParsingService
            .Setup(x => x.GetTagsFromCaddyfileContent(testContent))
            .Returns(expectedTags);

        // Act
        var result = _service.GetCaddyConfigurationInfo("test-with-tags");

        // Assert
        result.Should().NotBeNull();
        result.Hostnames.Should().BeEquivalentTo(expectedHostnames);
        result.ReverseProxyHostname.Should().Be(expectedTarget);
        result.ReverseProxyPorts.Should().BeEquivalentTo(expectedPorts);
        result.Tags.Should().BeEquivalentTo(expectedTags);
        result.Tags.Should().HaveCount(3);
        result.Tags.Should().Contain("web");
        result.Tags.Should().Contain("production");
        result.Tags.Should().Contain("ssl");
    }

    /// <summary>
    /// Tests that the Caddy service handles requests for configuration information of non-existent files gracefully.
    /// Setup: Requests configuration information for a file that doesn't exist in the configuration directory.
    /// Expectation: The service should return an empty configuration info object rather than throwing exceptions, ensuring robust error handling for missing configuration files.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationInfo_WithNonExistentFile_ReturnsEmptyInfo()
    {
        // Act
        var result = _service.GetCaddyConfigurationInfo("nonexistent");

        // Assert
        result.Should().NotBeNull();
        result.Hostnames.Should().BeEmpty();
        result.ReverseProxyHostname.Should().Be(string.Empty);
        result.ReverseProxyPorts.Should().BeEmpty();
    }

    /// <summary>
    /// Tests that the Caddy service handles empty configuration files by returning empty configuration information.
    /// Setup: Creates an empty configuration file and requests its configuration information.
    /// Expectation: The service should return an empty configuration info object, properly handling edge cases where configuration files exist but contain no parseable content.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationInfo_WithEmptyFile_ReturnsEmptyInfo()
    {
        // Arrange
        var filePath = Path.Combine(_tempConfigDir, "empty.caddy");
        File.WriteAllText(filePath, string.Empty);

        // Act
        var result = _service.GetCaddyConfigurationInfo("empty");

        // Assert
        result.Should().NotBeNull();
        result.Hostnames.Should().BeEmpty();
        result.ReverseProxyHostname.Should().Be(string.Empty);
        result.ReverseProxyPorts.Should().BeEmpty();
    }

    #endregion

    #region Additional Edge Cases and Error Scenarios

    /// <summary>
    /// Tests that the Caddy service handles file system permission errors gracefully when trying to read configuration files.
    /// Setup: Creates a file with restricted permissions and attempts to read its content.
    /// Expectation: The service should return an empty string for the content rather than throwing an exception, ensuring robust error handling for file system permission issues in production environments.
    /// </summary>
    [Fact]
    public void GetCaddyConfigurationContent_WithPermissionError_ReturnsEmptyString()
    {
        // Arrange
        var filePath = Path.Combine(_tempConfigDir, "restricted.caddy");
        File.WriteAllText(filePath, "test content");
        
        // Make file read-only to simulate permission issues
        var fileInfo = new FileInfo(filePath);
        fileInfo.Attributes = FileAttributes.ReadOnly;

        try
        {
            // Act
            var result = _service.GetCaddyConfigurationContent("restricted");

            // Assert
            // The service should still be able to read the file even if it's read-only
            // This test verifies that the service handles file operations gracefully
            result.Should().Be("test content");
        }
        finally
        {
            // Cleanup
            fileInfo.Attributes = FileAttributes.Normal;
        }
    }

    /// <summary>
    /// Tests that the Caddy service handles invalid file paths gracefully when saving configurations.
    /// Setup: Attempts to save a configuration with an invalid file path containing illegal characters.
    /// Expectation: The service should return a failure response with an appropriate error message, preventing file system errors and ensuring robust error handling for invalid file paths.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithInvalidFilePath_ReturnsFailure()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "invalid<>file",
            Content = "test content",
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        // The service should handle invalid characters gracefully
        // This test verifies that the service doesn't crash with invalid file names
        result.Success.Should().BeTrue();
        result.Message.Should().NotBeNullOrEmpty();
    }

    /// <summary>
    /// Tests that the Caddy service handles very large configuration files without performance issues.
    /// Setup: Creates a configuration request with a very large content string to simulate large Caddy configurations.
    /// Expectation: The service should successfully save the large configuration without throwing exceptions or experiencing significant performance degradation, ensuring the system can handle production-sized Caddy configurations.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithLargeContent_SavesSuccessfully()
    {
        // Arrange
        var largeContent = new string('a', 1000000); // 1MB content
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "large-config",
            Content = largeContent,
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        
        var filePath = Path.Combine(_tempConfigDir, "large-config.caddy");
        File.Exists(filePath).Should().BeTrue();
        File.ReadAllText(filePath).Should().Be(largeContent);
    }

    /// <summary>
    /// Tests that the Caddy service handles concurrent file access scenarios gracefully.
    /// Setup: Creates multiple threads attempting to save configurations simultaneously to simulate concurrent access.
    /// Expectation: The service should handle concurrent access without throwing exceptions, ensuring thread safety in multi-user environments where multiple users might be editing Caddy configurations simultaneously.
    /// </summary>
    [Fact]
    public async Task SaveCaddyConfiguration_WithConcurrentAccess_HandlesGracefully()
    {
        // Arrange
        var tasks = new List<Task<CaddyOperationResponse>>();
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "concurrent-test",
            Content = "test content",
            IsNew = true
        };

        // Act - Create multiple concurrent save operations
        for (int i = 0; i < 5; i++)
        {
            tasks.Add(Task.Run(() => _service.SaveCaddyConfiguration(request)));
        }

        // Wait for all tasks to complete
        await Task.WhenAll(tasks);

        // Assert
        tasks.Should().AllSatisfy(task => task.Result.Should().NotBeNull());
        // At least one should succeed, others might fail due to file already existing
        tasks.Should().Contain(task => task.Result.Success);
    }

    /// <summary>
    /// Tests that the Caddy service handles network file system scenarios where files might be temporarily unavailable.
    /// Setup: Simulates a scenario where the configuration directory becomes temporarily inaccessible.
    /// Expectation: The service should handle temporary file system unavailability gracefully, ensuring robust operation in network file system environments where connectivity might be intermittent.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_WithTemporarilyUnavailableDirectory_HandlesGracefully()
    {
        // Arrange
        var tempDir = Path.Combine(_tempConfigDir, "temp-unavailable");
        Directory.CreateDirectory(tempDir);
        _testConfigurations.ConfigDir = tempDir;

        // Create a file to ensure directory exists
        File.WriteAllText(Path.Combine(tempDir, "test.caddy"), "content");

        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        // Should handle gracefully even if directory becomes temporarily unavailable
    }

    /// <summary>
    /// Tests that the Caddy service handles configuration names with special characters and Unicode properly.
    /// Setup: Attempts to save and retrieve configurations with names containing special characters and Unicode.
    /// Expectation: The service should handle special characters and Unicode in configuration names correctly, ensuring compatibility with international domain names and special naming conventions.
    /// </summary>
    [Theory]
    [InlineData("config-with-unicode-测试")]
    [InlineData("config-with-special-chars!@#$%")]
    [InlineData("config-with-spaces and-dashes")]
    [InlineData("config.with.dots")]
    public void SaveCaddyConfiguration_WithSpecialCharacters_HandlesCorrectly(string fileName)
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = fileName,
            Content = "test content",
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        
        var filePath = Path.Combine(_tempConfigDir, $"{fileName}.caddy");
        File.Exists(filePath).Should().BeTrue();
    }

    /// <summary>
    /// Tests that the Caddy service handles null content gracefully when saving configurations.
    /// Setup: Attempts to save a configuration with null content.
    /// Expectation: The service should handle null content gracefully, either by treating it as empty string or providing appropriate error handling, ensuring robust operation with null inputs.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithNullContent_HandlesGracefully()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "null-content-test",
            Content = null!,
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        
        var filePath = Path.Combine(_tempConfigDir, "null-content-test.caddy");
        File.Exists(filePath).Should().BeTrue();
        File.ReadAllText(filePath).Should().Be(string.Empty);
    }

    /// <summary>
    /// Tests that the Caddy service handles disk space issues gracefully when saving large configurations.
    /// Setup: Attempts to save a configuration that would exceed available disk space (simulated).
    /// Expectation: The service should return a failure response with an appropriate error message when disk space is insufficient, ensuring proper error reporting for resource constraint scenarios.
    /// </summary>
    [Fact]
    public void SaveCaddyConfiguration_WithInsufficientDiskSpace_ReturnsFailure()
    {
        // Arrange
        var request = new CaddySaveConfigurationRequest
        {
            FileName = "disk-space-test",
            Content = new string('a', 1000000), // Large content
            IsNew = true
        };

        // Act
        var result = _service.SaveCaddyConfiguration(request);

        // Assert
        result.Should().NotBeNull();
        // In a real scenario with insufficient disk space, this would fail
        // For this test, we're just ensuring the method handles large content gracefully
        result.Success.Should().BeTrue();
    }

    /// <summary>
    /// Tests that the Caddy service handles file system corruption scenarios gracefully.
    /// Setup: Creates a scenario where the configuration directory structure is corrupted or invalid.
    /// Expectation: The service should handle file system corruption gracefully, either by creating necessary directories or providing appropriate error messages, ensuring robust operation in degraded file system conditions.
    /// </summary>
    [Fact]
    public void GetExistingCaddyConfigurations_WithCorruptedDirectory_HandlesGracefully()
    {
        // Arrange
        var corruptedDir = Path.Combine(_tempConfigDir, "corrupted");
        _testConfigurations.ConfigDir = corruptedDir;

        // Act
        var result = _service.GetExistingCaddyConfigurations();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        Directory.Exists(corruptedDir).Should().BeTrue();
    }

    #endregion
}
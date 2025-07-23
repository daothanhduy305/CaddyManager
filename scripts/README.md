# Test and Coverage Scripts

This folder contains scripts to run tests and generate coverage reports for the CaddyManager application.

## Available Scripts

### Bash Script (Linux/macOS)
- **File**: `run-tests-with-coverage.sh`
- **Usage**: `./scripts/run-tests-with-coverage.sh [OPTIONS]`

### PowerShell Script (Windows)
- **File**: `run-tests-with-coverage.ps1`
- **Usage**: `.\scripts\run-tests-with-coverage.ps1 [MODE]`

## Features

Both scripts provide the following functionality:

1. **Automatic Tool Installation**: Installs necessary .NET tools if not already present
2. **PATH Management**: Automatically adds .NET tools to PATH
3. **Test Execution**: Runs the complete test suite
4. **Coverage Collection**: Collects code coverage data
5. **Report Generation**: Creates HTML coverage reports
6. **Summary Statistics**: Provides coverage metrics

## Usage Options

### Full Mode (Default)
Runs tests with coverage collection and generates a detailed report.

```bash
# Bash (Linux/macOS)
./scripts/run-tests-with-coverage.sh

# PowerShell (Windows)
.\scripts\run-tests-with-coverage.ps1
```

### Tests Only Mode
Runs tests quickly without coverage collection (faster execution).

```bash
# Bash (Linux/macOS)
./scripts/run-tests-with-coverage.sh --tests-only

# PowerShell (Windows)
.\scripts\run-tests-with-coverage.ps1 tests-only
```

### Coverage Only Mode
Generates coverage report from existing coverage data.

```bash
# Bash (Linux/macOS)
./scripts/run-tests-with-coverage.sh --coverage-only

# PowerShell (Windows)
.\scripts\run-tests-with-coverage.ps1 coverage-only
```

### Help
Shows usage information and available options.

```bash
# Bash (Linux/macOS)
./scripts/run-tests-with-coverage.sh --help

# PowerShell (Windows)
.\scripts\run-tests-with-coverage.ps1 help
```

## Prerequisites

The scripts will automatically install the following tools if not present:

- **coverlet.collector**: For code coverage collection
- **dotnet-reportgenerator-globaltool**: For generating HTML reports

## Output

### Test Results
- Test execution status and results
- Pass/fail statistics
- Execution time

### Coverage Data
- **File**: `coverage.cobertura.xml`
- **Format**: Cobertura XML format
- **Location**: Project root directory

### Coverage Report
- **Directory**: `coverage-report/`
- **Main File**: `coverage-report/index.html`
- **Format**: Interactive HTML report
- **Features**: 
  - Line-by-line coverage details
  - Branch coverage information
  - File-level statistics
  - Search and filter capabilities

## Coverage Summary

The scripts provide a summary of coverage metrics:

- **Line Coverage**: Percentage of code lines executed
- **Branch Coverage**: Percentage of code branches executed
- **File Coverage**: Coverage statistics per file

## Troubleshooting

### Common Issues

1. **Permission Denied** (Bash script)
   ```bash
   chmod +x scripts/run-tests-with-coverage.sh
   ```

2. **PowerShell Execution Policy** (PowerShell script)
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

3. **No Coverage Data Generated**
   - This is normal if no code was executed during tests
   - Check that tests are actually running the code under test

4. **Tools Not Found**
   - The scripts will automatically install missing tools
   - Ensure you have .NET SDK installed

### Manual Tool Installation

If automatic installation fails, you can install tools manually:

```bash
# Install coverage collector
dotnet tool install --global coverlet.collector

# Install report generator
dotnet tool install --global dotnet-reportgenerator-globaltool
```

## Integration with CI/CD

These scripts can be integrated into CI/CD pipelines:

```yaml
# Example GitHub Actions step
- name: Run Tests with Coverage
  run: ./scripts/run-tests-with-coverage.sh --tests-only

- name: Generate Coverage Report
  run: ./scripts/run-tests-with-coverage.sh --coverage-only
```

## Customization

The scripts can be customized by modifying:

- **Coverage collection options**: Modify the `dotnet test` command parameters
- **Report generation**: Change report types and output formats
- **Tool installation**: Add or remove required tools
- **Output directories**: Modify coverage and report directories

## Support

For issues or questions about the scripts:

1. Check the troubleshooting section above
2. Review the script output for error messages
3. Ensure all prerequisites are met
4. Verify .NET SDK is properly installed 
# Test and Coverage Runner Script for CaddyManager (PowerShell)
# This script installs necessary tools, runs tests, and generates coverage reports

param(
    [Parameter(Position=0)]
    [ValidateSet("full", "tests-only", "coverage-only", "help")]
    [string]$Mode = "full"
)

# Function to write colored output
function Write-Status {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Function to check if a command exists
function Test-Command {
    param([string]$Command)
    try {
        Get-Command $Command -ErrorAction Stop | Out-Null
        return $true
    }
    catch {
        return $false
    }
}

# Function to add .NET tools to PATH
function Add-DotNetToolsToPath {
    $toolsPath = "$env:USERPROFILE\.dotnet\tools"
    if ($env:PATH -notlike "*$toolsPath*") {
        $env:PATH = "$env:PATH;$toolsPath"
        Write-Status "Added .NET tools to PATH: $toolsPath"
    }
}

# Function to install .NET tool if not already installed
function Install-DotNetTool {
    param(
        [string]$ToolName,
        [string]$ToolCommand
    )
    
    if (-not (Test-Command $ToolCommand)) {
        Write-Status "Installing $ToolName..."
        dotnet tool install --global $ToolName
        Write-Success "$ToolName installed successfully"
    }
    else {
        Write-Status "$ToolName is already installed"
    }
}

# Function to run tests with coverage
function Run-TestsWithCoverage {
    $coverageDir = "coverage-results"
    $coverageFile = "coverage.cobertura.xml"
    
    Write-Status "Running tests with coverage collection..."
    
    # Clean up previous coverage results
    if (Test-Path $coverageDir) {
        Remove-Item -Recurse -Force $coverageDir
    }
    if (Test-Path $coverageFile) {
        Remove-Item -Force $coverageFile
    }
    
    # Run tests with coverage using runsettings
    dotnet test --settings coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory $coverageDir --verbosity normal
    
    # Check if coverage file was generated in TestResults directory
    $latestCoverageFile = Get-ChildItem -Path "./CaddyManager.Tests/TestResults" -Name "coverage.cobertura.xml" -Recurse | Sort-Object LastWriteTime | Select-Object -Last 1
    
    if ($latestCoverageFile -and (Test-Path $latestCoverageFile)) {
        # Copy the latest coverage file to root directory
        Copy-Item $latestCoverageFile $coverageFile
        Write-Success "Coverage data collected: $coverageFile"
    }
    else {
        Write-Warning "No coverage data was generated. This might be normal if no code was executed."
    }
}

# Function to generate coverage report
function Generate-CoverageReport {
    $coverageFile = "coverage.cobertura.xml"
    $reportDir = "coverage-report"
    
    if (-not (Test-Path $coverageFile)) {
        Write-Warning "No coverage file found. Skipping report generation."
        return
    }
    
    Write-Status "Generating coverage report..."
    
    # Clean up previous report
    if (Test-Path $reportDir) {
        Remove-Item -Recurse -Force $reportDir
    }
    
    # Generate HTML report
    reportgenerator -reports:$coverageFile -targetdir:$reportDir -reporttypes:Html
    
    if (Test-Path $reportDir) {
        Write-Success "Coverage report generated in: $reportDir"
        Write-Status "Open $reportDir/index.html in your browser to view the report"
    }
    else {
        Write-Error "Failed to generate coverage report"
    }
}

# Function to generate coverage summary
function Generate-CoverageSummary {
    $coverageFile = "coverage.cobertura.xml"
    
    if (-not (Test-Path $coverageFile)) {
        Write-Warning "No coverage file found. Cannot generate summary."
        return
    }
    
    Write-Status "Generating coverage summary..."
    
    # Try to extract coverage information from XML
    try {
        $xml = [xml](Get-Content $coverageFile)
        $lineRate = $xml.coverage.line-rate
        $branchRate = $xml.coverage.branch-rate
        
        Write-Host ""
        Write-Host "=== COVERAGE SUMMARY ===" -ForegroundColor Cyan
        Write-Host "Line Coverage: $lineRate"
        Write-Host "Branch Coverage: $branchRate"
        Write-Host "========================" -ForegroundColor Cyan
        Write-Host ""
    }
    catch {
        Write-Warning "Could not extract coverage metrics from XML file."
    }
}

# Function to run tests without coverage (for faster execution)
function Run-TestsOnly {
    Write-Status "Running tests only (no coverage collection)..."
    dotnet test --verbosity normal
}

# Function to show help
function Show-Help {
    Write-Host "Usage: .\run-tests-with-coverage.ps1 [MODE]"
    Write-Host ""
    Write-Host "Modes:"
    Write-Host "  full           Run tests with coverage and generate report (default)"
    Write-Host "  tests-only     Run tests without coverage collection (faster)"
    Write-Host "  coverage-only  Generate coverage report from existing data"
    Write-Host "  help           Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\run-tests-with-coverage.ps1              # Run full test suite with coverage"
    Write-Host "  .\run-tests-with-coverage.ps1 tests-only   # Run tests quickly without coverage"
    Write-Host "  .\run-tests-with-coverage.ps1 coverage-only # Generate report from existing coverage data"
}

# Main script execution
function Main {
    Write-Host "==========================================" -ForegroundColor Cyan
    Write-Host "CaddyManager Test and Coverage Runner" -ForegroundColor Cyan
    Write-Host "==========================================" -ForegroundColor Cyan
    Write-Host ""
    
    # Handle help mode
    if ($Mode -eq "help") {
        Show-Help
        return
    }
    
    # Add .NET tools to PATH
    Add-DotNetToolsToPath
    
    # Install necessary tools
    Write-Status "Checking and installing necessary tools..."
    Install-DotNetTool "coverlet.collector" "coverlet"
    Install-DotNetTool "dotnet-reportgenerator-globaltool" "reportgenerator"
    
    # Run based on mode
    switch ($Mode) {
        "tests-only" {
            Run-TestsOnly
        }
        "coverage-only" {
            Generate-CoverageReport
            Generate-CoverageSummary
        }
        "full" {
            Run-TestsWithCoverage
            Generate-CoverageReport
            Generate-CoverageSummary
        }
    }
    
    Write-Host ""
    Write-Success "Script execution completed!"
    
    if ($Mode -ne "tests-only" -and (Test-Path "coverage-report")) {
        Write-Host ""
        Write-Host "Next steps:" -ForegroundColor Yellow
        Write-Host "1. Open coverage-report/index.html in your browser"
        Write-Host "2. Review the detailed coverage report"
        Write-Host "3. Check for any uncovered code paths"
    }
}

# Run main function
Main 
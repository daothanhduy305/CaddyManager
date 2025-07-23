#!/bin/bash

# Test and Coverage Runner Script for CaddyManager
# This script installs necessary tools, runs tests, and generates coverage reports

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to add .NET tools to PATH
add_dotnet_tools_to_path() {
    local tools_path="$HOME/.dotnet/tools"
    if [[ ":$PATH:" != *":$tools_path:"* ]]; then
        export PATH="$PATH:$tools_path"
        print_status "Added .NET tools to PATH: $tools_path"
    fi
}

# Function to install .NET tool if not already installed
install_dotnet_tool() {
    local tool_name=$1
    local tool_command=$2
    
    if ! command_exists "$tool_command"; then
        print_status "Installing $tool_name..."
        dotnet tool install --global "$tool_name"
        print_success "$tool_name installed successfully"
    else
        print_status "$tool_name is already installed"
    fi
}

# Function to run tests with coverage
run_tests_with_coverage() {
    local coverage_dir="coverage-results"
    local coverage_file="coverage.cobertura.xml"
    
    print_status "Running tests with coverage collection..."
    
    # Clean up previous coverage results
    if [ -d "$coverage_dir" ]; then
        rm -rf "$coverage_dir"
    fi
    if [ -f "$coverage_file" ]; then
        rm -f "$coverage_file"
    fi
    
    # Run tests with coverage using runsettings
    dotnet test --settings CaddyManager.Tests/coverlet.runsettings --collect:"XPlat Code Coverage" --results-directory "$coverage_dir" --verbosity normal
    
    # Check if coverage file was generated in TestResults directory
    local latest_coverage_file=$(find ./CaddyManager.Tests/TestResults -name "coverage.cobertura.xml" -type f -printf '%T@ %p\n' | sort -n | tail -1 | cut -f2- -d" ")
    
    if [ -n "$latest_coverage_file" ] && [ -f "$latest_coverage_file" ]; then
        # Copy the latest coverage file to root directory
        cp "$latest_coverage_file" "$coverage_file"
        print_success "Coverage data collected: $coverage_file"
    else
        print_warning "No coverage data was generated. This might be normal if no code was executed."
    fi
}

# Function to generate coverage report
generate_coverage_report() {
    local coverage_file="coverage.cobertura.xml"
    local report_dir="coverage-report"
    
    if [ ! -f "$coverage_file" ]; then
        print_warning "No coverage file found. Skipping report generation."
        return
    fi
    
    print_status "Generating coverage report..."
    
    # Clean up previous report
    if [ -d "$report_dir" ]; then
        rm -rf "$report_dir"
    fi
    
    # Generate HTML report
    reportgenerator -reports:"$coverage_file" -targetdir:"$report_dir" -reporttypes:Html -sourcedirs:"CaddyManager.Contracts;CaddyManager.Services"
    
    if [ -d "$report_dir" ]; then
        print_success "Coverage report generated in: $report_dir"
        print_status "Open $report_dir/index.html in your browser to view the report"
    else
        print_error "Failed to generate coverage report"
    fi
}

# Function to generate coverage summary
generate_coverage_summary() {
    local coverage_file="coverage.cobertura.xml"
    
    if [ ! -f "$coverage_file" ]; then
        print_warning "No coverage file found. Cannot generate summary."
        return
    fi
    
    print_status "Generating coverage summary..."
    
    # Try to extract coverage information from XML
    if command_exists "xmllint"; then
        local line_rate=$(xmllint --xpath "string(//coverage/@line-rate)" "$coverage_file" 2>/dev/null || echo "N/A")
        local branch_rate=$(xmllint --xpath "string(//coverage/@branch-rate)" "$coverage_file" 2>/dev/null || echo "N/A")
        
        echo ""
        echo "=== COVERAGE SUMMARY ==="
        echo "Line Coverage: $line_rate"
        echo "Branch Coverage: $branch_rate"
        echo "========================"
        echo ""
    else
        print_warning "xmllint not available. Cannot extract coverage metrics."
    fi
}

# Function to run tests without coverage (for faster execution)
run_tests_only() {
    print_status "Running tests only (no coverage collection)..."
    dotnet test --verbosity normal
}

# Function to show help
show_help() {
    echo "Usage: $0 [OPTIONS]"
    echo ""
    echo "Options:"
    echo "  --tests-only     Run tests without coverage collection (faster)"
    echo "  --coverage-only  Generate coverage report from existing data"
    echo "  --full           Run tests with coverage and generate report (default)"
    echo "  --help           Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0                    # Run full test suite with coverage"
    echo "  $0 --tests-only       # Run tests quickly without coverage"
    echo "  $0 --coverage-only    # Generate report from existing coverage data"
}

# Main script execution
main() {
    echo "=========================================="
    echo "CaddyManager Test and Coverage Runner"
    echo "=========================================="
    echo ""
    
    # Parse command line arguments
    local mode="full"
    while [[ $# -gt 0 ]]; do
        case $1 in
            --tests-only)
                mode="tests-only"
                shift
                ;;
            --coverage-only)
                mode="coverage-only"
                shift
                ;;
            --full)
                mode="full"
                shift
                ;;
            --help)
                show_help
                exit 0
                ;;
            *)
                print_error "Unknown option: $1"
                show_help
                exit 1
                ;;
        esac
    done
    
    # Add .NET tools to PATH
    add_dotnet_tools_to_path
    
    # Install necessary tools
    print_status "Checking and installing necessary tools..."
    install_dotnet_tool "coverlet.collector" "coverlet"
    install_dotnet_tool "dotnet-reportgenerator-globaltool" "reportgenerator"
    
    # Run based on mode
    case $mode in
        "tests-only")
            run_tests_only
            ;;
        "coverage-only")
            generate_coverage_report
            generate_coverage_summary
            ;;
        "full")
            run_tests_with_coverage
            generate_coverage_report
            generate_coverage_summary
            ;;
    esac
    
    echo ""
    print_success "Script execution completed!"
    
    if [ "$mode" != "tests-only" ] && [ -d "coverage-report" ]; then
        echo ""
        echo "Next steps:"
        echo "1. Open coverage-report/index.html in your browser"
        echo "2. Review the detailed coverage report"
        echo "3. Check for any uncovered code paths"
    fi
}

# Run main function with all arguments
main "$@" 
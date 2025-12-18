<#
.SYNOPSIS
    Publishes the Harvest NuGet package to nuget.org.

.DESCRIPTION
    This script performs the following steps:
    1. Checks for uncommitted changes (git porcelain)
    2. Determines the Nerdbank git version
    3. Validates nuget-key.txt exists, has content, and is gitignored
    4. Runs unit tests (unless -SkipTests is specified)
    5. Publishes to nuget.org

.PARAMETER SkipTests
    Skip running unit tests before publishing.

.EXAMPLE
    .\Publish.ps1
    .\Publish.ps1 -SkipTests
#>

param(
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'

function Write-Step {
    param([string]$Message)
    Write-Host "`n=== $Message ===" -ForegroundColor Cyan
}

function Exit-WithError {
    param([string]$Message)
    Write-Host "ERROR: $Message" -ForegroundColor Red
    exit 1
}

# Step 1: Check for uncommitted changes
Write-Step "Checking for uncommitted changes"

$gitStatus = git status --porcelain
if ($gitStatus) {
    Exit-WithError "Working directory is not clean. Please commit or stash your changes.`n$gitStatus"
}
Write-Host "Working directory is clean." -ForegroundColor Green

# Step 2: Determine Nerdbank git version
Write-Step "Determining Nerdbank git version"

$versionOutput = nbgv get-version --format json 2>&1
if ($LASTEXITCODE -ne 0) {
    Exit-WithError "Failed to get Nerdbank git version. Ensure nbgv is installed (dotnet tool install -g nbgv).`n$versionOutput"
}

$versionInfo = $versionOutput | ConvertFrom-Json
$version = $versionInfo.NuGetPackageVersion
if (-not $version) {
    Exit-WithError "Could not determine NuGet package version from Nerdbank.GitVersioning."
}
Write-Host "Version: $version" -ForegroundColor Green

# Step 3: Validate nuget-key.txt
Write-Step "Validating nuget-key.txt"

$nugetKeyPath = Join-Path $PSScriptRoot "nuget-key.txt"

if (-not (Test-Path $nugetKeyPath)) {
    Exit-WithError "nuget-key.txt not found at: $nugetKeyPath"
}

$nugetKey = (Get-Content $nugetKeyPath -Raw).Trim()
if ([string]::IsNullOrWhiteSpace($nugetKey)) {
    Exit-WithError "nuget-key.txt is empty."
}

# Check if nuget-key.txt is gitignored
$gitCheckIgnore = git check-ignore "nuget-key.txt" 2>&1
if ($LASTEXITCODE -ne 0) {
    Exit-WithError "nuget-key.txt is NOT gitignored. Add it to .gitignore before publishing."
}
Write-Host "nuget-key.txt is valid and gitignored." -ForegroundColor Green

# Step 4: Run unit tests (unless skipped)
if ($SkipTests) {
    Write-Step "Skipping unit tests"
    Write-Host "Tests skipped by user request." -ForegroundColor Yellow
} else {
    Write-Step "Running unit tests"
    
    dotnet test "$PSScriptRoot\Harvest.Test\Harvest.Test.csproj" --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Exit-WithError "Unit tests failed."
    }
    Write-Host "All tests passed." -ForegroundColor Green
}

# Step 5: Build and publish to nuget.org
Write-Step "Building package"

$projectPath = Join-Path $PSScriptRoot "Harvest\Harvest.csproj"
dotnet pack $projectPath --configuration Release --no-restore
if ($LASTEXITCODE -ne 0) {
    Exit-WithError "Failed to build NuGet package."
}

$packagePath = Join-Path $PSScriptRoot "Harvest\bin\Release\Harvest.$version.nupkg"
if (-not (Test-Path $packagePath)) {
    # Try to find the package with a wildcard in case version format differs
    $packagePath = Get-ChildItem -Path (Join-Path $PSScriptRoot "Harvest\bin\Release") -Filter "Harvest.*.nupkg" | 
                   Sort-Object LastWriteTime -Descending | 
                   Select-Object -First 1 -ExpandProperty FullName
    
    if (-not $packagePath) {
        Exit-WithError "Could not find NuGet package in Harvest\bin\Release\"
    }
}
Write-Host "Package created: $packagePath" -ForegroundColor Green

Write-Step "Publishing to nuget.org"

dotnet nuget push $packagePath --api-key $nugetKey --source https://api.nuget.org/v3/index.json --skip-duplicate
if ($LASTEXITCODE -ne 0) {
    Exit-WithError "Failed to publish package to nuget.org."
}

Write-Host "`n=== Successfully published Harvest $version to nuget.org ===" -ForegroundColor Green
exit 0

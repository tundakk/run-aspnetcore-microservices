# Pre-deployment setup script
# This script prepares the environment for Azure deployment

param(
    [Parameter()]
    [string]$SubscriptionId = "",
    
    [Parameter()]
    [switch]$SkipLogin
)

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

Write-Host "🔧 EmailIntelligence Azure Deployment Setup" -ForegroundColor Cyan
Write-Host ("=" * 50)

# Check prerequisites
Write-Status "Checking prerequisites..."

# Check Azure CLI
if (!(Get-Command az -ErrorAction SilentlyContinue)) {
    Write-Error "Azure CLI is not installed. Please install it from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
}
Write-Success "Azure CLI is installed"

# Check Docker
if (!(Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Error "Docker is not installed. Please install Docker Desktop."
    exit 1
}
Write-Success "Docker is installed"

# Check Docker is running
try {
    docker info | Out-Null
    Write-Success "Docker is running"
} catch {
    Write-Error "Docker is not running. Please start Docker Desktop."
    exit 1
}

# Check .NET SDK
if (!(Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Error ".NET SDK is not installed. Please install .NET 8.0 SDK."
    exit 1
}
Write-Success ".NET SDK is installed"

# Azure authentication
if (-not $SkipLogin) {
    Write-Status "Checking Azure authentication..."
    try {
        $currentAccount = az account show 2>$null | ConvertFrom-Json
        if ($currentAccount) {
            Write-Success "Already logged in to Azure as: $($currentAccount.user.name)"
            Write-Host "Current subscription: $($currentAccount.name) ($($currentAccount.id))" -ForegroundColor Gray
        } else {
            throw "Not logged in"
        }
    } catch {
        Write-Status "Logging in to Azure..."
        az login
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Azure login failed"
            exit 1
        }
    }
    
    # Set subscription if provided
    if ($SubscriptionId) {
        Write-Status "Setting subscription to: $SubscriptionId"
        az account set --subscription $SubscriptionId
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Failed to set subscription"
            exit 1
        }
        Write-Success "Subscription set successfully"
    }
}

# Verify project structure
Write-Status "Verifying project structure..."
$scriptDir = Split-Path -Parent $PSCommandPath
$projectRoot = Split-Path -Parent $scriptDir
$emailIntelligenceProject = Join-Path $projectRoot "src\Services\EmailIntelligence\EmailIntelligence.API\EmailIntelligence.API.csproj"

if (Test-Path $emailIntelligenceProject) {
    Write-Success "EmailIntelligence project found"
} else {
    Write-Error "EmailIntelligence project not found at: $emailIntelligenceProject"
    exit 1
}

# Build and test the project
Write-Status "Building EmailIntelligence project..."
$emailIntelligenceDir = Join-Path $projectRoot "src\Services\EmailIntelligence"
Set-Location $emailIntelligenceDir

try {
    dotnet restore
    if ($LASTEXITCODE -ne 0) { throw "Restore failed" }
    Write-Success "Package restore completed"
    
    dotnet build --configuration Release --no-restore
    if ($LASTEXITCODE -ne 0) { throw "Build failed" }
    Write-Success "Build completed successfully"
    
} catch {
    Write-Error "Failed to build EmailIntelligence project: $_"
    exit 1
} finally {
    Set-Location $scriptDir
}

# Test Docker build
Write-Status "Testing Docker build..."
try {
    Set-Location $emailIntelligenceDir
    docker build -t emailintelligence-api-test:latest -f EmailIntelligence.API/Dockerfile . --quiet
    if ($LASTEXITCODE -ne 0) { throw "Docker build failed" }
    Write-Success "Docker build test completed"
    
    # Clean up test image
    docker rmi emailintelligence-api-test:latest --force | Out-Null
} catch {
    Write-Error "Docker build test failed: $_"
    exit 1
} finally {
    Set-Location $scriptDir
}

# Validate deployment files
Write-Status "Validating deployment files..."
$requiredFiles = @(
    "main.bicep",
    "parameters.json",
    "deploy.ps1"
)

foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Success "$file exists"
    } else {
        Write-Error "$file is missing"
        exit 1
    }
}

# Check parameters.json for default password
$parametersContent = Get-Content "parameters.json" | ConvertFrom-Json
if ($parametersContent.parameters.postgresAdminPassword.value -eq "ChangeMe123!") {
    Write-Warning "Default PostgreSQL password detected in parameters.json"
    Write-Host "Please update the password before deployment for security." -ForegroundColor Yellow
}

# Final checks
Write-Status "Running final checks..."

# Check if local services are running (optional)
try {
    Invoke-RestMethod -Uri "http://localhost:6006/health" -TimeoutSec 5 -ErrorAction Stop | Out-Null
    Write-Success "Local EmailIntelligence API is running and healthy"
} catch {
    Write-Warning "Local EmailIntelligence API is not running. This is optional for deployment."
}

Write-Host "`n" + ("=" * 50)
Write-Host "✅ Setup completed successfully!" -ForegroundColor Green
Write-Host "`nYou're ready to deploy to Azure! Run:" -ForegroundColor Cyan
Write-Host "  .\deploy.ps1" -ForegroundColor White

if ($SubscriptionId) {
    Write-Host "  (Subscription $SubscriptionId is already set)" -ForegroundColor Gray
} else {
    Write-Host "  (Add -SubscriptionId parameter if needed)" -ForegroundColor Gray
}

Write-Host "`nOr for manual deployment:" -ForegroundColor Cyan
Write-Host "  az deployment group create --resource-group rg-emailintelligence-dev --template-file main.bicep --parameters parameters.json" -ForegroundColor White

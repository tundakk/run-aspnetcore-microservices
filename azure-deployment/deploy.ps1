# Azure deployment script for EmailIntelligence service (PowerShell version)
# This script deploys the EmailIntelligence microservice to Azure Container Apps

param(
    [string]$SubscriptionId = "",
    [string]$ResourceGroupName = "rg-emailintelligence-dev",
    [string]$Location = "eastus"
)

# Configuration
$DeploymentName = "emailintelligence-deployment-$(Get-Date -Format 'yyyyMMdd-HHmmss')"

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

try {
    Write-Status "Starting Azure deployment for EmailIntelligence service..."

    # Check if Azure CLI is installed
    if (!(Get-Command az -ErrorAction SilentlyContinue)) {
        Write-Error "Azure CLI is not installed. Please install it first."
        exit 1
    }

    # Login to Azure (if not already logged in)
    Write-Status "Checking Azure authentication..."
    $accountInfo = az account show 2>$null | ConvertFrom-Json
    if (!$accountInfo) {
        Write-Status "Please log in to Azure..."
        az login
    }

    # Set subscription if provided
    if ($SubscriptionId) {
        Write-Status "Setting subscription to $SubscriptionId..."
        az account set --subscription $SubscriptionId
    }

    # Create resource group
    Write-Status "Creating resource group $ResourceGroupName..."
    az group create --name $ResourceGroupName --location $Location

    # Deploy infrastructure
    Write-Status "Deploying infrastructure using Bicep template..."
    $deploymentOutput = az deployment group create `
        --resource-group $ResourceGroupName `
        --name $DeploymentName `
        --template-file "main.bicep" `
        --parameters "parameters.json" `
        --output json | ConvertFrom-Json

    if ($LASTEXITCODE -eq 0) {
        Write-Success "Infrastructure deployment completed successfully!"
        
        # Extract outputs
        $containerRegistryName = $deploymentOutput.properties.outputs.containerRegistryName.value
        $containerRegistryLoginServer = $deploymentOutput.properties.outputs.containerRegistryLoginServer.value
        $appUrl = $deploymentOutput.properties.outputs.emailIntelligenceAppUrl.value
        
        Write-Success "Container Registry: $containerRegistryName"
        Write-Success "Registry Login Server: $containerRegistryLoginServer"
        Write-Success "Application URL: $appUrl"
        
        # Build and push Docker image
        Write-Status "Building and pushing Docker image..."
        
        # Get registry credentials
        $registryUsername = az acr credential show --name $containerRegistryName --query "username" --output tsv
        $registryPassword = az acr credential show --name $containerRegistryName --query "passwords[0].value" --output tsv
          # Login to container registry
        Write-Status "Logging into container registry..."
        Write-Output $registryPassword | docker login $containerRegistryLoginServer --username $registryUsername --password-stdin
        
        # Build image from the EmailIntelligence directory
        Write-Status "Building Docker image..."
        Set-Location "../src/Services/EmailIntelligence"
        docker build -t "$containerRegistryLoginServer/emailintelligence-api:latest" -f "EmailIntelligence.API/Dockerfile" .
        
        # Push image
        Write-Status "Pushing Docker image to registry..."
        docker push "$containerRegistryLoginServer/emailintelligence-api:latest"
        
        # Update container app with new image
        Write-Status "Updating container app..."
        az containerapp update `
            --name "ca-emailintelligence-api-dev" `
            --resource-group $ResourceGroupName `
            --image "$containerRegistryLoginServer/emailintelligence-api:latest"
        
        Write-Success "Deployment completed successfully!"
        Write-Success "Your EmailIntelligence API is available at: $appUrl"
        Write-Status "It may take a few minutes for the application to be fully ready."
        
        # Test the health endpoint
        Write-Status "Testing health endpoint..."
        Start-Sleep -Seconds 30
          try {
            Invoke-RestMethod -Uri "$appUrl/health" -Method Get -TimeoutSec 30 | Out-Null
            Write-Success "Health check passed! API is responding."
        }
        catch {
            Write-Warning "Health check failed, but this might be temporary. Please check the app logs in Azure portal."
        }
        
    } else {
        Write-Error "Infrastructure deployment failed!"
        exit 1
    }
}
catch {
    Write-Error "An error occurred during deployment: $($_.Exception.Message)"
    exit 1
}
finally {
    # Return to original directory
    Set-Location $PSScriptRoot
}

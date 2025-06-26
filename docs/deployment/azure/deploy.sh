#!/bin/bash

# Azure deployment script for EmailIntelligence service
# This script deploys the EmailIntelligence microservice to Azure Container Apps

set -e

# Configuration
RESOURCE_GROUP_NAME="rg-emailintelligence-dev"
LOCATION="eastus"
SUBSCRIPTION_ID=""  # Set your subscription ID here
DEPLOYMENT_NAME="emailintelligence-deployment-$(date +%Y%m%d-%H%M%S)"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

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

# Check if Azure CLI is installed
if ! command -v az &> /dev/null; then
    print_error "Azure CLI is not installed. Please install it first."
    exit 1
fi

print_status "Starting Azure deployment for EmailIntelligence service..."

# Login to Azure (if not already logged in)
print_status "Checking Azure authentication..."
if ! az account show &> /dev/null; then
    print_status "Please log in to Azure..."
    az login
fi

# Set subscription if provided
if [ ! -z "$SUBSCRIPTION_ID" ]; then
    print_status "Setting subscription to $SUBSCRIPTION_ID..."
    az account set --subscription $SUBSCRIPTION_ID
fi

# Create resource group
print_status "Creating resource group $RESOURCE_GROUP_NAME..."
az group create --name $RESOURCE_GROUP_NAME --location $LOCATION

# Deploy infrastructure
print_status "Deploying infrastructure using Bicep template..."
DEPLOYMENT_OUTPUT=$(az deployment group create \
    --resource-group $RESOURCE_GROUP_NAME \
    --name $DEPLOYMENT_NAME \
    --template-file main.bicep \
    --parameters parameters.json \
    --output json)

if [ $? -eq 0 ]; then
    print_success "Infrastructure deployment completed successfully!"
    
    # Extract outputs
    CONTAINER_REGISTRY_NAME=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.containerRegistryName.value')
    CONTAINER_REGISTRY_LOGIN_SERVER=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.containerRegistryLoginServer.value')
    APP_URL=$(echo $DEPLOYMENT_OUTPUT | jq -r '.properties.outputs.emailIntelligenceAppUrl.value')
    
    print_success "Container Registry: $CONTAINER_REGISTRY_NAME"
    print_success "Registry Login Server: $CONTAINER_REGISTRY_LOGIN_SERVER"
    print_success "Application URL: $APP_URL"
    
    # Build and push Docker image
    print_status "Building and pushing Docker image..."
    
    # Get registry credentials
    REGISTRY_USERNAME=$(az acr credential show --name $CONTAINER_REGISTRY_NAME --query "username" --output tsv)
    REGISTRY_PASSWORD=$(az acr credential show --name $CONTAINER_REGISTRY_NAME --query "passwords[0].value" --output tsv)
    
    # Login to container registry
    echo $REGISTRY_PASSWORD | docker login $CONTAINER_REGISTRY_LOGIN_SERVER --username $REGISTRY_USERNAME --password-stdin
    
    # Build image from the EmailIntelligence directory
    print_status "Building Docker image..."
    cd ../src/Services/EmailIntelligence
    docker build -t $CONTAINER_REGISTRY_LOGIN_SERVER/emailintelligence-api:latest -f EmailIntelligence.API/Dockerfile .
    
    # Push image
    print_status "Pushing Docker image to registry..."
    docker push $CONTAINER_REGISTRY_LOGIN_SERVER/emailintelligence-api:latest
    
    # Update container app with new image
    print_status "Updating container app..."
    az containerapp update \
        --name ca-emailintelligence-api-dev \
        --resource-group $RESOURCE_GROUP_NAME \
        --image $CONTAINER_REGISTRY_LOGIN_SERVER/emailintelligence-api:latest
    
    print_success "Deployment completed successfully!"
    print_success "Your EmailIntelligence API is available at: $APP_URL"
    print_status "It may take a few minutes for the application to be fully ready."
    
else
    print_error "Infrastructure deployment failed!"
    exit 1
fi

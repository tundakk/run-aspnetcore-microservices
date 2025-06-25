# EmailIntelligence Service - Azure Deployment

This directory contains the Azure deployment resources for the EmailIntelligence microservice using Azure Container Apps and Azure Database for PostgreSQL.

## Prerequisites

1. **Azure CLI**: Install the Azure CLI from [here](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
2. **Docker**: Ensure Docker is installed and running
3. **Azure Subscription**: You need an active Azure subscription
4. **PowerShell** (Windows) or **Bash** (Linux/macOS)

## Architecture

The deployment creates the following Azure resources:

- **Azure Container Apps Environment**: Hosting environment for the containerized application
- **Azure Container Registry**: Private registry for storing Docker images
- **Azure Database for PostgreSQL Flexible Server**: Managed PostgreSQL database
- **Log Analytics Workspace**: For application logs and monitoring

## Deployment Steps

### 1. Prepare for Deployment

First, ensure your EmailIntelligence service is built and running locally:

```bash
# Navigate to the project root
cd c:\Repositories\run-aspnetcore-microservices\src

# Ensure the service is built
docker-compose build emailintelligence.api
```

### 2. Configure Parameters

Edit the `parameters.json` file to customize your deployment:

```json
{
  "location": "East US",           # Azure region
  "environmentSuffix": "dev",      # Environment name (dev/test/prod)
  "postgresAdminPassword": "YourSecurePassword123!"  # Change this!
}
```

**Important**: Change the default PostgreSQL password before deployment!

### 3. Deploy to Azure

#### Option A: PowerShell (Windows)

```powershell
# Navigate to the deployment directory
cd azure-deployment

# Run the deployment script
.\deploy.ps1 -SubscriptionId "your-subscription-id"

# Or without specifying subscription (will use current default)
.\deploy.ps1
```

#### Option B: Bash (Linux/macOS)

```bash
# Navigate to the deployment directory
cd azure-deployment

# Make the script executable
chmod +x deploy.sh

# Edit the script to set your subscription ID
# Then run the deployment
./deploy.sh
```

### 4. Manual Deployment (Alternative)

If you prefer to deploy manually:

```bash
# Login to Azure
az login

# Create resource group
az group create --name rg-emailintelligence-dev --location eastus

# Deploy using Bicep
az deployment group create \
  --resource-group rg-emailintelligence-dev \
  --template-file main.bicep \
  --parameters parameters.json
```

## Post-Deployment

### 1. Verify Deployment

After deployment completes, you'll receive:
- **Application URL**: The public endpoint for your API
- **Container Registry**: Name of the created registry
- **Database Server**: PostgreSQL server details

### 2. Test the API

```bash
# Test the health endpoint
curl https://your-app-url/health

# Test the API endpoints
curl https://your-app-url/api/emailintelligence/analyze
```

### 3. Monitor the Application

- **Azure Portal**: Check Container Apps logs and metrics
- **Log Analytics**: Query application logs
- **Application Insights**: (Optional) Add for advanced monitoring

## Configuration

### Environment Variables

The deployed application uses these environment variables:

- `ASPNETCORE_ENVIRONMENT`: Set to "Production"
- `ConnectionStrings__DefaultConnection`: PostgreSQL connection string
- `ConnectionStrings__EmailIntelligenceDb`: Same as DefaultConnection
- `ASPNETCORE_URLS`: Set to "http://+:8080"

### Database

The PostgreSQL database is automatically created with:
- **Server**: PostgreSQL 15 Flexible Server
- **SKU**: Standard_B1ms (Burstable tier)
- **Storage**: 32GB
- **Backup**: 7-day retention

### Scaling

The Container App is configured with:
- **Min Replicas**: 1
- **Max Replicas**: 3
- **Scaling Rule**: HTTP-based (10 concurrent requests per instance)

## Troubleshooting

### Common Issues

1. **Build Failures**
   - Ensure Docker is running
   - Check that all NuGet packages are restored
   - Verify the Dockerfile exists and is correct

2. **Database Connection Issues**
   - Check firewall rules (Azure services should be allowed)
   - Verify connection string format
   - Ensure PostgreSQL server is running

3. **Container App Not Starting**
   - Check container logs in Azure Portal
   - Verify environment variables are set correctly
   - Check health probe configuration

### Getting Logs

```bash
# Get container app logs
az containerapp logs show \
  --name ca-emailintelligence-api-dev \
  --resource-group rg-emailintelligence-dev

# Follow logs in real-time
az containerapp logs show \
  --name ca-emailintelligence-api-dev \
  --resource-group rg-emailintelligence-dev \
  --follow
```

## Cleanup

To remove all Azure resources:

```bash
az group delete --name rg-emailintelligence-dev --yes --no-wait
```

## Cost Optimization

- **PostgreSQL**: Use Burstable tier for development
- **Container Apps**: Configure appropriate min/max replicas
- **Container Registry**: Use Basic tier for small projects
- **Log Analytics**: Set appropriate retention period

## Security Considerations

1. **Secrets Management**: Use Azure Key Vault for production
2. **Network Security**: Consider VNet integration for production
3. **Authentication**: Add Azure AD authentication
4. **SSL/TLS**: HTTPS is enabled by default
5. **Database Security**: PostgreSQL requires SSL connections

## Next Steps

1. **Add Monitoring**: Integrate Application Insights
2. **CI/CD Pipeline**: Set up GitHub Actions or Azure DevOps
3. **Environment Promotion**: Create staging and production environments
4. **API Documentation**: Add Swagger/OpenAPI documentation
5. **Load Testing**: Test the API under load

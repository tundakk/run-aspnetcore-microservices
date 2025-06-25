# 🔐 API Key Configuration

## Important Security Notice

The EmailIntelligence service requires an OpenAI API key to function. For security reasons, the actual API key is not included in the repository.

**🔒 Security Configuration:** Development appsettings files and docker-compose override files are now included in `.gitignore` to prevent accidentally committing sensitive configuration data.

## Setting Up Your API Key

### Method 1: Environment Variables (Recommended for Production)
Set the following environment variable:
```bash
export LLMSettings__ApiKey="your-actual-openai-api-key"
```

### Method 2: User Secrets (Recommended for Development)
```bash
dotnet user-secrets set "LLMSettings:ApiKey" "your-actual-openai-api-key" --project src/Services/EmailIntelligence/EmailIntelligence.API
```

### Method 3: Configuration Files (Development Only)
Replace `your-openai-api-key-here` in the following files with your actual API key:

1. `src/Services/EmailIntelligence/EmailIntelligence.API/appsettings.json`
2. `src/Services/EmailIntelligence/EmailIntelligence.API/appsettings.Development.json`
3. `src/docker-compose.override.yml`

⚠️ **WARNING**: Never commit actual API keys to version control!

## Getting an OpenAI API Key

1. Visit [OpenAI Platform](https://platform.openai.com/)
2. Sign up or log in to your account
3. Navigate to API Keys section
4. Create a new API key
5. Copy the key and use it in your configuration

## Docker Compose Setup

For Docker deployment, update the environment variable in `docker-compose.override.yml`:
```yaml
environment:
  - LLMSettings__ApiKey=your-actual-openai-api-key
```

## Azure Deployment

For Azure deployment, configure the API key in:
- Azure App Service Configuration
- Azure Key Vault (recommended)
- Environment variables in the deployment script

## Git Security Configuration

The following files are now included in `.gitignore` to prevent accidentally committing sensitive data:

```gitignore
# Development appsettings that may contain sensitive data
**/appsettings.Development.json
**/appsettings.Local.json
**/appsettings.*.json
!**/appsettings.json
!**/appsettings.Production.json

# Docker override files with sensitive environment variables
docker-compose.override.yml

# Environment files
.env
.env.local
.env.development
.env.production

# API keys and secrets
secrets.json
*.secrets.json
```

This ensures that any development configuration files containing API keys or other sensitive data won't be accidentally committed to the repository.

# 🛠️ Setup & Configuration

This section contains comprehensive setup guides for getting the EShop Microservices platform running in your development environment.

## 📋 Setup Guides

### 🚀 [EmailIntelligence Setup](emailintelligence-setup.md)
Complete setup guide for the EmailIntelligence microservice including Docker debugging, VS Code configuration, and API testing.

### 🔐 [API Key Configuration](api-key-setup.md)  
Security configuration guide for setting up OpenAI API keys and managing sensitive configuration data.

### 🌱 [Seed Data Documentation](seed-data-documentation.md)
Comprehensive documentation of the seed data included with the EmailIntelligence service for testing and development.

## 🎯 Quick Start

### **Prerequisites**
Before starting, ensure you have:
- ✅ [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running
- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
- ✅ [Visual Studio Code](https://code.visualstudio.com/) with C# extensions
- ✅ [Git](https://git-scm.com/) for version control

### **System Requirements**
- **OS**: Windows 10/11, macOS, or Linux
- **RAM**: 8 GB minimum (16 GB recommended)
- **CPU**: 4 cores minimum
- **Storage**: 20 GB free space
- **Docker**: 4 GB memory allocation minimum

## 🏃‍♂️ 3-Step Quick Setup

### **Step 1: Clone and Navigate**
```powershell
git clone https://github.com/aspnetrun/run-aspnetcore-microservices.git
cd run-aspnetcore-microservices
```

### **Step 2: Configure API Key**
```powershell
# Set OpenAI API key (required for EmailIntelligence service)
$env:LLMSettings__ApiKey = "your-openai-api-key-here"

# Or use .NET User Secrets (recommended for development)
dotnet user-secrets set "LLMSettings:ApiKey" "your-openai-api-key" --project src/Services/EmailIntelligence/EmailIntelligence.API
```

### **Step 3: Start Services**
```powershell
# Start all microservices with Docker Compose
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Verify services are running
docker ps

# Access the application
# Web UI: http://localhost:6005
# API Gateway: http://localhost:6004
```

## 🔧 Development Environment Setup

### **Visual Studio Code Configuration**
1. **Install Required Extensions:**
   - C# for Visual Studio Code
   - Docker Extension
   - REST Client (for API testing)

2. **Open Workspace:**
   ```powershell
   code .
   ```

3. **Debug Configuration:**
   The workspace includes pre-configured debug settings in `.vscode/launch.json` for debugging microservices in Docker containers.

### **Database Setup**
All databases are automatically configured and initialized via Docker Compose:

- **PostgreSQL** (Catalog, Basket, EmailIntelligence)
- **SQL Server** (Ordering)
- **Redis** (Distributed Cache)
- **SQLite** (Discount - embedded)

### **Message Broker Setup**
RabbitMQ is automatically configured with:
- **AMQP Port**: 5672
- **Management UI**: http://localhost:15672
- **Credentials**: guest/guest

## 🧪 Testing Setup

### **API Testing**
Import the provided API collections:
- **Insomnia**: [`docs/collections/EmailIntelligence-Insomnia-Collection.json`](../collections/EmailIntelligence-Insomnia-Collection.json)
- **Postman**: [`docs/collections/EShopMicroservices.postman_collection.json`](../collections/EShopMicroservices.postman_collection.json)

### **Health Checks**
Verify all services are healthy:
```powershell
# Check individual services
curl http://localhost:6000/health    # Catalog
curl http://localhost:6001/health    # Basket  
curl http://localhost:6002/health    # Discount
curl http://localhost:6003/health    # Ordering
curl http://localhost:6006/health    # EmailIntelligence

# Check via API Gateway
curl http://localhost:6004/catalog-service/health
curl http://localhost:6004/basket-service/health
curl http://localhost:6004/emailintelligence-service/health
```

## 🔐 Security Configuration

### **Development Security**
- Default credentials for databases
- HTTP endpoints (HTTPS in production)
- Basic API key authentication
- Local container networking

### **Production Security Checklist**
- [ ] Change all default passwords
- [ ] Enable HTTPS/TLS everywhere
- [ ] Use proper secrets management (Azure Key Vault, etc.)
- [ ] Configure network security groups
- [ ] Enable audit logging
- [ ] Set up monitoring and alerting

## 📊 Monitoring Setup

### **Application Monitoring**
- Health check endpoints on all services
- Structured logging with correlation IDs
- Performance counters and metrics
- Error tracking and alerting

### **Infrastructure Monitoring**
- Docker container health checks
- Database connection monitoring
- Message broker queue monitoring
- Resource utilization tracking

## 🛠️ Development Tools

### **Recommended Extensions**
```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.csdevkit", 
    "ms-azuretools.vscode-docker",
    "humao.rest-client",
    "eamodio.gitlens",
    "rangav.vscode-thunder-client"
  ]
}
```

### **VS Code Tasks**
Pre-configured tasks available via `Ctrl+Shift+P` → "Tasks: Run Task":
- 🚀 Run EmailIntelligence with Docker Compose
- 🔨 Build EmailIntelligence
- 🔄 Rebuild EmailIntelligence Docker
- ⏹️ Stop Docker Services
- 📊 View Docker Logs
- 🐳 Show Running Containers

## 🔄 Configuration Management

### **Environment Variables**
Key configuration settings in `docker-compose.override.yml`:

```yaml
environment:
  # Database connections
  - ConnectionStrings__Database=Server=emailintelligencedb;Port=5432;Database=EmailIntelligenceDb;User Id=postgres;Password=postgres
  
  # Message broker
  - MessageBroker__Host=amqp://ecommerce-mq:5672
  - MessageBroker__UserName=guest
  - MessageBroker__Password=guest
  
  # AI service configuration
  - LLMSettings__ApiKey=your-openai-api-key
  - LLMSettings__BaseUrl=https://api.openai.com
  - LLMSettings__Model=gpt-3.5-turbo
```

### **Configuration Validation**
The application includes configuration validation to ensure all required settings are present and valid at startup.

## 🚨 Troubleshooting

### **Common Setup Issues**

#### **Docker Desktop Not Running**
```powershell
# Check Docker status
docker version

# Start Docker Desktop if not running
# Windows: Start Docker Desktop from Start Menu
# macOS: Open Docker Desktop from Applications
# Linux: sudo systemctl start docker
```

#### **Port Conflicts**
```powershell
# Check for port conflicts
netstat -ano | findstr :6006

# Stop conflicting services or change ports in docker-compose.override.yml
```

#### **Memory Issues**
```powershell
# Increase Docker Desktop memory allocation
# Docker Desktop → Settings → Resources → Memory: 8GB+

# Check current usage
docker stats
```

#### **API Key Issues**
- Verify OpenAI API key is valid
- Check environment variable is properly set
- Ensure no extra spaces or characters in the key
- Verify the key has sufficient credits/quota

### **Getting Help**
- Review the [Debugging Guide](../guides/debugging-guide.md)
- Check the [Testing Guide](../testing/testing-guide.md)
- Consult the [Architecture Documentation](../architecture/README.md)
- Open an issue in the repository with detailed error information

## 🎉 Success Indicators

You'll know your setup is complete when:
- ✅ All Docker containers are running (`docker ps`)
- ✅ Health checks return 200 OK status
- ✅ Web UI loads at http://localhost:6005
- ✅ API Gateway responds at http://localhost:6004
- ✅ Database connections are successful
- ✅ RabbitMQ management UI accessible at http://localhost:15672
- ✅ EmailIntelligence API can process test emails
- ✅ All services can communicate with each other

## 🚀 Next Steps

After completing the setup:
1. **Explore the Architecture**: Review the [architecture documentation](../architecture/README.md)
2. **Test the APIs**: Import and run the [API collections](../collections/README.md)
3. **Debug Services**: Set up [debugging in VS Code](../guides/debugging-guide.md)
4. **Run Tests**: Follow the [testing guide](../testing/testing-guide.md)
5. **Deploy to Production**: Consider [Azure deployment](../deployment/azure-deployment.md)

Welcome to the EShop Microservices platform! 🎯

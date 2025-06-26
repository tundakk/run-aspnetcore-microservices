# 🚀 Deployment Guide

This section contains comprehensive deployment guides for various environments and platforms.

## 📋 Deployment Options

### 🐳 [Docker Deployment](docker-deployment.md)
Deploy the entire application stack using Docker Compose for local development and testing.

### ☁️ [Azure Deployment](azure-deployment.md)  
Complete Azure cloud deployment with Container Apps, managed databases, and production-ready infrastructure.

### 🏭 [Production Configuration](production-config.md)
Production-ready configuration guidelines, security considerations, and best practices.

## 🎯 Quick Start Options

### **Option 1: Local Development**
```powershell
# Start all services locally
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Verify services are running
docker ps

# Access web application
# http://localhost:6005
```

### **Option 2: Azure Cloud**
```powershell
# Navigate to Azure deployment
cd docs/deployment/azure

# Run deployment script
./deploy.ps1 -ResourceGroupName "eshop-rg" -Location "eastus"
```

## 🌍 Environment Overview

| Environment | Use Case | Infrastructure | Database | Scaling |
|-------------|----------|----------------|----------|---------|
| **Development** | Local coding | Docker Compose | Container DBs | Manual |
| **Testing** | CI/CD Pipeline | Docker/K8s | Managed DBs | Auto |
| **Staging** | Pre-production | Azure Container Apps | Azure SQL/PostgreSQL | Auto |
| **Production** | Live system | Azure/AWS/GCP | Managed DBs | Auto + Manual |

## 🔧 Infrastructure Requirements

### **Minimum System Requirements**
- **CPU**: 4 cores
- **RAM**: 8 GB
- **Storage**: 50 GB free space
- **Docker**: Latest version
- **OS**: Windows 10/11, macOS, or Linux

### **Recommended Production Setup**
- **CPU**: 8+ cores per service
- **RAM**: 16+ GB per service
- **Storage**: SSD with high IOPS
- **Load Balancer**: Azure Load Balancer or equivalent
- **CDN**: Azure CDN for static assets

## 🗄️ Database Deployment

### **Development (Docker)**
```yaml
services:
  catalogdb:
    image: postgres:15
    environment:
      POSTGRES_DB: CatalogDb
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
    ports:
      - "5432:5432"
```

### **Production (Azure)**
- **Azure Database for PostgreSQL**: Catalog, Basket, EmailIntelligence
- **Azure SQL Database**: Ordering service
- **Azure Cache for Redis**: Basket caching
- **Azure Service Bus**: Message broker (alternative to RabbitMQ)

## 🔐 Security Configuration

### **Development Security**
- Default credentials (change in production)
- HTTP endpoints (add HTTPS in production)
- Basic validation
- Local-only access

### **Production Security**
- **SSL/TLS**: HTTPS everywhere
- **Authentication**: Azure AD or OAuth 2.0
- **Authorization**: Role-based access control (RBAC)
- **Secrets**: Azure Key Vault or equivalent
- **Network**: Virtual networks and security groups
- **Monitoring**: Azure Monitor and Application Insights

## 📊 Monitoring & Logging

### **Application Monitoring**
```yaml
# Application Insights configuration
ApplicationInsights:
  InstrumentationKey: "your-key-here"
  
# Health check endpoints
HealthChecks:
  Enabled: true
  Endpoints:
    - /health
    - /health/ready
    - /health/live
```

### **Infrastructure Monitoring**
- **Container Health**: Docker health checks
- **Resource Usage**: CPU, memory, storage
- **Network**: Latency, throughput, errors
- **Database**: Connection pools, query performance

## 🚀 CI/CD Pipeline

### **GitHub Actions Workflow**
```yaml
name: Deploy EShop Microservices

on:
  push:
    branches: [main]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Build Docker images
      run: docker-compose build
    - name: Deploy to Azure
      run: ./scripts/deploy-azure.sh
```

### **Deployment Stages**
1. **Source Control**: Git triggers
2. **Build**: Docker image creation
3. **Test**: Automated testing
4. **Deploy**: Environment promotion
5. **Verify**: Health checks and smoke tests

## 🔄 Backup & Recovery

### **Database Backups**
- **Automated**: Daily database backups
- **Point-in-time**: Recovery to specific timestamp
- **Cross-region**: Geo-redundant storage
- **Testing**: Regular restore verification

### **Application Backups**
- **Configuration**: Infrastructure as Code (IaC)
- **Secrets**: Secure backup of certificates and keys
- **Static Assets**: CDN and blob storage backups

## 📈 Scaling Strategies

### **Horizontal Scaling**
```yaml
# Container scaling configuration
services:
  catalog.api:
    deploy:
      replicas: 3
      resources:
        limits:
          cpus: '1.0'
          memory: 1G
```

### **Auto Scaling Triggers**
- **CPU Usage**: > 70% for 5 minutes
- **Memory Usage**: > 80% for 5 minutes  
- **Request Queue**: > 100 pending requests
- **Response Time**: > 2 seconds average

## 🛠️ Troubleshooting

### **Common Deployment Issues**
- **Port Conflicts**: Check port availability
- **Database Connections**: Verify connection strings
- **Memory Limits**: Increase container memory
- **Health Check Failures**: Check service dependencies

### **Debug Commands**
```powershell
# Check service status
docker-compose ps

# View service logs
docker-compose logs -f [service-name]

# Execute commands in container
docker exec -it [container-name] bash

# Check resource usage
docker stats
```

## 📞 Support & Maintenance

### **Monitoring Alerts**
- **Service Down**: Immediate notification
- **High Error Rate**: > 5% error rate
- **Performance Degradation**: Response time > 3 seconds
- **Resource Usage**: > 90% utilization

### **Maintenance Windows**
- **Scheduled**: Weekly maintenance window
- **Emergency**: On-demand for critical issues
- **Zero-downtime**: Rolling updates when possible
- **Rollback**: Automated rollback on failure

---

Choose the deployment option that best fits your needs and environment. Each option includes detailed step-by-step instructions and troubleshooting guides.

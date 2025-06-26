# 🐳 Docker Deployment Guide

This guide covers deploying the EShop Microservices application using Docker and Docker Compose for local development and testing environments.

## 📋 Prerequisites

### **Required Software**
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (latest version)
- [Git](https://git-scm.com/) for source control
- **Minimum System Requirements:**
  - 8 GB RAM
  - 4 CPU cores  
  - 20 GB free disk space

### **Verify Installation**
```powershell
# Check Docker version
docker --version
docker-compose --version

# Verify Docker is running
docker ps
```

## 🚀 Quick Start Deployment

### **Step 1: Clone Repository**
```powershell
git clone https://github.com/aspnetrun/run-aspnetcore-microservices.git
cd run-aspnetcore-microservices
```

### **Step 2: Configure API Keys**
Follow the [API Key Setup Guide](../setup/api-key-setup.md) to configure your OpenAI API key for the EmailIntelligence service.

### **Step 3: Start All Services**
```powershell
# Start all services in detached mode
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# View startup logs
docker-compose logs -f
```

### **Step 4: Verify Deployment**
```powershell
# Check all containers are running
docker ps

# Test service health
curl http://localhost:6006/health        # EmailIntelligence
curl http://localhost:6000/health        # Catalog
curl http://localhost:6001/health        # Basket
curl http://localhost:6003/health        # Ordering
```

## 🏗️ Service Architecture

### **Container Stack**
```
🌐 Web UI (port 6005)
    ↓
🚪 API Gateway (port 6004) 
    ↓
🔀 Microservices:
├── 📦 Catalog API (port 6000)
├── 🛒 Basket API (port 6001) 
├── 💰 Discount gRPC (port 6002)
├── 📋 Ordering API (port 6003)
└── 🧠 EmailIntelligence API (port 6006)
    ↓
🗄️ Databases:
├── 📊 PostgreSQL (Catalog - port 5432)
├── 📊 PostgreSQL (Basket - port 5433)  
├── 📊 PostgreSQL (EmailIntelligence - port 5434)
├── 🗃️ SQLite (Discount - embedded)
├── 🗄️ SQL Server (Ordering - port 1433)
└── ⚡ Redis (Cache - port 6379)
    ↓
📨 RabbitMQ (ports 5672, 15672)
```

## 🔧 Service Configuration

### **Environment Variables**
All services are configured via environment variables in `docker-compose.override.yml`:

```yaml
emailintelligence.api:
  environment:
    - ASPNETCORE_ENVIRONMENT=Development
    - ConnectionStrings__Database=Server=emailintelligencedb;Port=5432;Database=EmailIntelligenceDb;User Id=postgres;Password=postgres
    - LLMSettings__ApiKey=your-openai-api-key
    - MessageBroker__Host=amqp://ecommerce-mq:5672
```

### **Port Mapping**
| Service | Container Port | Host Port | Protocol |
|---------|---------------|-----------|----------|
| Web UI | 8080 | 6005 | HTTP |
| API Gateway | 8080 | 6004 | HTTP |
| Catalog API | 8080 | 6000 | HTTP |
| Basket API | 8080 | 6001 | HTTP |
| Discount gRPC | 8080 | 6002 | gRPC |
| Ordering API | 8080 | 6003 | HTTP |
| EmailIntelligence API | 8080 | 6006 | HTTP |
| PostgreSQL (Catalog) | 5432 | 5432 | TCP |
| PostgreSQL (Basket) | 5432 | 5433 | TCP |
| PostgreSQL (EmailIntelligence) | 5432 | 5434 | TCP |
| SQL Server | 1433 | 1433 | TCP |
| Redis | 6379 | 6379 | TCP |
| RabbitMQ | 5672 | 5672 | AMQP |
| RabbitMQ Management | 15672 | 15672 | HTTP |

## 🗄️ Database Configuration

### **PostgreSQL Services**
```yaml
catalogdb:
  image: postgres:15
  environment:
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres
    POSTGRES_DB: CatalogDb
  volumes:
    - postgres_catalog:/var/lib/postgresql/data/

basketdb:
  image: postgres:15
  environment:
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres  
    POSTGRES_DB: BasketDb
  volumes:
    - postgres_basket:/var/lib/postgresql/data/

emailintelligencedb:
  image: postgres:15
  environment:
    POSTGRES_USER: postgres
    POSTGRES_PASSWORD: postgres
    POSTGRES_DB: EmailIntelligenceDb
  volumes:
    - postgres_emailintelligence:/var/lib/postgresql/data/
    - ./Services/EmailIntelligence/init-db.sql:/docker-entrypoint-initdb.d/init-db.sql
```

### **SQL Server**
```yaml
orderdb:
  image: mcr.microsoft.com/mssql/server:2022-latest
  environment:
    ACCEPT_EULA: Y
    SA_PASSWORD: SwN12345678
```

## 📨 Message Broker

### **RabbitMQ Configuration**
```yaml
messagebroker:
  image: rabbitmq:3-management
  environment:
    RABBITMQ_DEFAULT_USER: guest
    RABBITMQ_DEFAULT_PASS: guest
  ports:
    - "5672:5672"    # AMQP port
    - "15672:15672"  # Management UI
```

**Management Interface**: http://localhost:15672
- Username: `guest`
- Password: `guest`

## 🔍 Health Monitoring

### **Health Check Endpoints**
```powershell
# Service health checks
curl http://localhost:6000/health        # Catalog
curl http://localhost:6001/health        # Basket  
curl http://localhost:6003/health        # Ordering
curl http://localhost:6006/health        # EmailIntelligence

# Infrastructure health
curl http://localhost:15672              # RabbitMQ Management
# Redis: redis-cli -p 6379 ping
```

### **Database Connections**
```powershell
# PostgreSQL connections
docker exec -it catalogdb psql -U postgres -d CatalogDb
docker exec -it basketdb psql -U postgres -d BasketDb  
docker exec -it emailintelligencedb psql -U postgres -d EmailIntelligenceDb

# SQL Server connection
docker exec -it orderdb /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SwN12345678

# Redis connection
docker exec -it distributedcache redis-cli
```

## 🛠️ Management Commands

### **Service Management**
```powershell
# Start all services
docker-compose up -d

# Start specific service
docker-compose up -d emailintelligence.api

# Stop all services
docker-compose down

# Stop and remove volumes (clean slate)
docker-compose down -v

# Restart specific service  
docker-compose restart emailintelligence.api

# View service logs
docker-compose logs -f emailintelligence.api

# Scale specific service
docker-compose up -d --scale catalog.api=3
```

### **Build Commands**
```powershell
# Build all images
docker-compose build

# Build specific service
docker-compose build emailintelligence.api

# Build without cache
docker-compose build --no-cache emailintelligence.api

# Pull latest base images
docker-compose pull
```

## 🔧 Troubleshooting

### **Common Issues**

#### **Port Already in Use**
```powershell
# Find process using port
netstat -ano | findstr :6006

# Kill process by PID
taskkill /PID <process-id> /F

# Or change port in docker-compose.override.yml
```

#### **Container Startup Failures**
```powershell
# Check container status
docker ps -a

# View container logs
docker logs <container-name>

# Inspect container configuration
docker inspect <container-name>

# Check resource usage
docker stats
```

#### **Database Connection Issues**
```powershell
# Verify database container is running
docker ps | grep postgres

# Check database logs
docker logs emailintelligencedb

# Test connection manually
docker exec -it emailintelligencedb psql -U postgres -d EmailIntelligenceDb -c "SELECT 1;"
```

#### **Memory Issues**
```powershell
# Check available memory
docker system df

# Clean up unused resources
docker system prune -f

# Remove unused volumes
docker volume prune -f

# Check container resource limits
docker stats --no-stream
```

### **Debug Mode Setup**
For debugging applications running in containers:

```yaml
# Add to docker-compose.override.yml
emailintelligence.api:
  environment:
    - DOTNET_USE_POLLING_FILE_WATCHER=1
  volumes:
    - ./src/Services/EmailIntelligence:/app/src
  ports:
    - "6006:8080"
    - "6007:8081"  # Debug port
```

## 📊 Performance Optimization

### **Resource Limits**
```yaml
# Add resource constraints
services:
  emailintelligence.api:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M
```

### **Volume Optimization**
```yaml
# Use named volumes for better performance
volumes:
  postgres_emailintelligence:
    driver: local
  
# Mount specific directories for development
volumes:
  - ./src/Services/EmailIntelligence:/app/src:cached
```

## 🔄 Backup & Recovery

### **Database Backups**
```powershell
# Backup PostgreSQL
docker exec -t emailintelligencedb pg_dump -U postgres EmailIntelligenceDb > backup.sql

# Restore PostgreSQL
cat backup.sql | docker exec -i emailintelligencedb psql -U postgres -d EmailIntelligenceDb

# Backup SQL Server
docker exec -t orderdb /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P SwN12345678 -Q "BACKUP DATABASE OrderDb TO DISK = '/tmp/orderdb.bak'"
```

### **Volume Backups**
```powershell
# Backup named volumes
docker run --rm -v postgres_emailintelligence:/data -v ${PWD}:/backup alpine tar czf /backup/emailintelligence-backup.tar.gz /data

# Restore named volumes  
docker run --rm -v postgres_emailintelligence:/data -v ${PWD}:/backup alpine tar xzf /backup/emailintelligence-backup.tar.gz -C /
```

## 🎯 Next Steps

After successful deployment:

1. **Test the Application**: Visit http://localhost:6005 to access the web UI
2. **Import API Collections**: Use the [Insomnia collection](../collections/README.md) for API testing
3. **Monitor Services**: Check logs and health endpoints regularly
4. **Development Setup**: Follow the [debugging guide](../guides/debugging-guide.md) for VS Code setup
5. **Production Deployment**: Consider [Azure deployment](azure-deployment.md) for production use

Your Docker-based EShop Microservices deployment is now ready for development and testing! 🚀

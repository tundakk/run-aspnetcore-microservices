# 🎯 EmailIntelligence Microservice - Complete Setup Guide

## 📋 **Quick Start Checklist**

### ✅ **Prerequisites Verified**
- [x] Docker Desktop installed and running
- [x] VS Code with C# extensions installed
- [x] .NET 8 SDK available
- [x] PowerShell terminal configured

### ✅ **Project Files Ready**
- [x] `EmailIntelligence-Insomnia-Collection.json` - API testing collection
- [x] `EmailIntelligence-Docker-Debugging-Guide.md` - Comprehensive debugging guide
- [x] `docker-compose.override.yml` - Updated for debugging support
- [x] `.vscode/launch.json` - VS Code debug configurations
- [x] `.vscode/tasks.json` - Build and Docker tasks
- [x] `.vscode/settings.json` - Optimal VS Code settings

## 🚀 **Getting Started (3 Steps)**

### **Step 1: Start the Services**
```powershell
# Navigate to project root
cd c:\Repositories\run-aspnetcore-microservices

# Start all services
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d

# Verify services are running
docker ps
```

### **Step 2: Test with Insomnia**
1. **Import Collection:** `EmailIntelligence-Insomnia-Collection.json`
2. **Setup Environment Variables:**
   - `base_url`: `http://localhost:6006`
   - `yarp_url`: `http://localhost:6000`
3. **Test endpoints** starting with the health check

### **Step 3: Debug in VS Code**
1. **Open VS Code** in project root
2. **Press F5** or go to Debug panel
3. **Select:** "🐛 Attach to EmailIntelligence (Docker)"
4. **Set breakpoints** and test with Insomnia

## 📁 **File Structure Overview**

```
c:\Repositories\run-aspnetcore-microservices\
├── 📄 EmailIntelligence-Insomnia-Collection.json    # API testing
├── 📄 EmailIntelligence-Docker-Debugging-Guide.md   # Complete guide
├── .vscode/
│   ├── 📄 launch.json      # Debug configurations
│   ├── 📄 tasks.json       # Build & Docker tasks
│   └── 📄 settings.json    # VS Code optimization
├── src/
│   ├── 📄 docker-compose.yml
│   ├── 📄 docker-compose.override.yml    # Debug-enabled
│   └── Services/EmailIntelligence/
│       ├── EmailIntelligence.API/
│       ├── EmailIntelligence.Application/
│       └── EmailIntelligence.Infrastructure/
```

## 🎪 **Available Debugging Options**

### **1. 🐛 Docker Attach (Recommended)**
- **Best for:** Production-like debugging
- **Setup:** Services run in Docker, debugger attaches
- **Use when:** Testing full microservice integration

### **2. 🔍 Local Debug**
- **Best for:** Rapid development
- **Setup:** Run locally, connect to Docker database
- **Use when:** Making frequent code changes

### **3. 🔥 Multi-Service Debug**
- **Best for:** Complex scenarios
- **Setup:** Debug multiple services simultaneously
- **Use when:** Debugging service interactions

## 📊 **Available VS Code Tasks**

Access via **Terminal > Run Task** or `Ctrl+Shift+P` → "Tasks: Run Task":

- 🚀 **Run EmailIntelligence with Docker Compose**
- 🔨 **Build EmailIntelligence**
- 🔄 **Rebuild EmailIntelligence Docker**
- ⏹️ **Stop Docker Services**
- 📊 **View Docker Logs**
- 🐳 **Show Running Containers**

## 🔗 **Service Endpoints**

### **Direct API Access:**
- **Base URL:** `http://localhost:6006`
- **Health:** `http://localhost:6006/health`
- **Swagger:** `http://localhost:6006/swagger`

### **Through YARP Gateway:**
- **Base URL:** `http://localhost:6000/emailintelligence-service`
- **Health:** `http://localhost:6000/emailintelligence-service/health`

## 🧪 **Testing Workflow**

### **1. Health Check**
```http
GET http://localhost:6006/health
```

### **2. Create Email Analytics**
```http
POST http://localhost:6006/api/v1/emailanalytics
Content-Type: application/json

{
  "emailId": "test@example.com",
  "subject": "Test Subject",
  "body": "Test email body for sentiment analysis",
  "recipientId": "user123",
  "campaignId": "campaign456"
}
```

### **3. Query Analytics**
```http
GET http://localhost:6006/api/v1/emailanalytics/campaign456
```

## 🐛 **Common Debugging Scenarios**

### **Setting Breakpoints:**
1. **Open** the EmailIntelligence service files
2. **Click** in the gutter next to line numbers
3. **Start debugging** (F5)
4. **Trigger** the endpoint with Insomnia

### **Variable Inspection:**
- **Hover** over variables during debugging
- **Use Watch panel** for complex expressions
- **Check Call Stack** for execution flow

### **Log Monitoring:**
```powershell
# Follow logs in real-time
docker logs -f src-emailintelligence.api-1

# All services logs
docker-compose logs -f
```

## 🔧 **Troubleshooting Quick Fixes**

### **"Container not found"**
```powershell
docker ps | findstr emailintelligence
# Update container name in launch.json if different
```

### **"Port already in use"**
```powershell
netstat -ano | findstr :6006
# Kill process or change port in docker-compose.override.yml
```

### **"Breakpoints not hitting"**
1. Verify container is running in Development mode
2. Check Debug Output panel in VS Code
3. Rebuild container: `docker-compose build emailintelligence.api`

## 📚 **Documentation Links**

- **Complete Guide:** `../guides/debugging-guide.md`
- **API Collection:** `../collections/insomnia-collection.json`
- **Docker Config:** `../src/docker-compose.override.yml`

## 🎉 **Success Indicators**

You're all set when you can:
- ✅ Start services with Docker Compose
- ✅ Import and run Insomnia collection
- ✅ Attach debugger and hit breakpoints
- ✅ Create and query email analytics
- ✅ Monitor logs and troubleshoot issues

## 🚀 **Next Steps**

1. **Explore the codebase** - Understanding the service architecture
2. **Add custom endpoints** - Extend the EmailIntelligence API
3. **Write unit tests** - Improve code quality and reliability
4. **Performance testing** - Use tools like NBomber or k6
5. **Production deployment** - Azure Container Apps or Kubernetes

---

**Happy Debugging! 🎯** Your EmailIntelligence microservice is now fully configured for development, testing, and debugging in VS Code with Docker support!

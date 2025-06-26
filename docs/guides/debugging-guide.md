# EmailIntelligence Docker Debugging Guide

## 🐛 **How to Debug EmailIntelligence Service in Docker**

### **Method 1: Remote Debugging with Visual Studio/VS Code (Recommended)**

#### **Prerequisites:**
- Visual Studio 2022 or VS Code with C# extension
- Docker Desktop running
- EmailIntelligence service running in Docker

#### **Step 1: Enable Remote Debugging**
✅ Already configured! The `docker-compose.override.yml` now includes:
- Development environment variables
- Volume mounts for debugging symbols
- Proper port mappings

#### **Step 2: Attach Debugger in Visual Studio**
1. **Open the EmailIntelligence solution** in Visual Studio
2. **Go to:** `Debug` → `Attach to Process...`
3. **Set Connection Type:** `Docker (Linux Container)`
4. **Set Connection Target:** `localhost:6006` (or your Docker host)
5. **Find the process:** Look for `EmailIntelligence.API` or `dotnet`
6. **Attach** and set breakpoints!

#### **Step 3: Setup VS Code for Docker Debugging**

##### **3.1 Install Required Extensions:**
1. **C# for Visual Studio Code** (ms-dotnettools.csharp)
2. **Docker** (ms-azuretools.vscode-docker)
3. **C# Dev Kit** (ms-dotnettools.csdevkit) - Optional but recommended

##### **3.2 Create VS Code Configuration:**
1. **Open the root workspace folder** in VS Code (`c:\Repositories\run-aspnetcore-microservices`)
2. **Create `.vscode/launch.json`** in the workspace root:
```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Attach to EmailIntelligence (Docker)",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickRemoteProcess}",
            "pipeTransport": {
                "pipeProgram": "docker",
                "pipeArgs": ["exec", "-i", "src-emailintelligence.api-1"],
                "debuggerPath": "/vsdbg/vsdbg",
                "pipeCwd": "${workspaceRoot}",
                "quoteArgs": false
            },
            "sourceFileMap": {
                "/src": "${workspaceRoot}/src"
            }
        },
        {
            "name": "Launch EmailIntelligence (Local)",
            "type": "coreclr",
            "request": "launch",
            "program": "${workspaceFolder}/src/Services/EmailIntelligence/EmailIntelligence.API/bin/Debug/net8.0/EmailIntelligence.API.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/Services/EmailIntelligence/EmailIntelligence.API",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development",
                "ASPNETCORE_HTTP_PORTS": "6006"
            },
            "console": "internalConsole"
        }
    ]
}
```

##### **3.3 VS Code Debugging Workflow:**

**Option A: Attach to Docker Container (Recommended)**
1. **Start the services:** Run the Docker Compose task or use terminal:
   ```powershell
   docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
   ```

2. **Open Debug Panel:** Press `Ctrl+Shift+D` or click Debug icon in sidebar

3. **Select configuration:** Choose "Attach to EmailIntelligence (Docker)" from dropdown

4. **Start debugging:** Press `F5` or click green play button

5. **Select process:** VS Code will show a list of processes - select the `dotnet` or `EmailIntelligence.API` process

6. **Set breakpoints:** Click in the gutter next to line numbers in your C# files

7. **Test endpoints:** Use your Insomnia collection to trigger the breakpoints

**Option B: Run Locally with Docker Dependencies**
1. **Stop EmailIntelligence container:**
   ```powershell
   docker-compose stop emailintelligence.api
   ```

2. **Keep dependencies running:**
   ```powershell
   docker-compose up -d emailintelligencedb messagebroker
   ```

3. **Select "Launch EmailIntelligence (Local)"** in debug dropdown

4. **Press F5** to start debugging locally

## 🎯 **VS Code Docker Debugging Setup - Step by Step**

### **Prerequisites:**
1. **VS Code Extensions:**
   - C# for Visual Studio Code (ms-dotnettools.csharp)
   - Docker (ms-azuretools.vscode-docker)
   - C# Dev Kit (ms-dotnettools.csdevkit) - Optional but recommended

2. **Verify Docker is Running:**
   ```powershell
   docker ps
   ```

### **Step 1: Open Project in VS Code**
1. **Open VS Code**
2. **File** → **Open Folder**
3. **Navigate to:** `c:\Repositories\run-aspnetcore-microservices`
4. **Click "Select Folder"**

### **Step 2: Create VS Code Configuration**
1. **Create `.vscode` folder** (if it doesn't exist):
   - Right-click in Explorer panel → **New Folder** → Name it `.vscode`

2. **Create `launch.json` file:**
   - Right-click `.vscode` folder → **New File** → Name it `launch.json`
   - **Copy and paste the configuration below:**

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "🐛 Attach to EmailIntelligence (Docker)",
            "type": "coreclr",
            "request": "attach",
            "processId": "${command:pickRemoteProcess}",
            "pipeTransport": {
                "pipeProgram": "docker",
                "pipeArgs": ["exec", "-i", "src-emailintelligence.api-1"],
                "debuggerPath": "/vsdbg/vsdbg",
                "pipeCwd": "${workspaceRoot}",
                "quoteArgs": false
            },
            "sourceFileMap": {
                "/src": "${workspaceRoot}/src"
            }
        },
        {
            "name": "🔍 Debug EmailIntelligence (Local)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/src/Services/EmailIntelligence/EmailIntelligence.API/bin/Debug/net8.0/EmailIntelligence.API.dll",
            "args": [],
            "cwd": "${workspaceFolder}/src/Services/EmailIntelligence/EmailIntelligence.API",
            "stopAtEntry": false,
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        }
    ]
}
```

### **Step 3: Start EmailIntelligence Service**
1. **Open Terminal in VS Code:** `Ctrl + `` (backtick)
2. **Run the service:**
   ```powershell
   docker-compose up -d emailintelligence.api
   ```
3. **Verify it's running:**
   ```powershell
   docker ps | findstr emailintelligence
   ```

### **Step 4: Attach Debugger to Docker Container**
1. **Open Run and Debug panel:** `Ctrl + Shift + D`
2. **Select configuration:** "🐛 Attach to EmailIntelligence (Docker)"
3. **Click the green play button** or press `F5`
4. **VS Code will show a dropdown** with running processes
5. **Look for and select:** `EmailIntelligence.API` or `dotnet` process
6. **Click to attach!**

### **Step 5: Set Breakpoints**
1. **Navigate to EmailIntelligence files:**
   ```
   src/Services/EmailIntelligence/EmailIntelligence.API/
   ├── Features/ProcessEmail/ProcessEmailEndpoint.cs
   ├── Features/ProcessEmail/ProcessEmailHandler.cs
   └── Features/DraftGeneration/GenerateDraftEndpoint.cs
   ```

2. **Set breakpoints by clicking in the left margin** (red dots):
   - **ProcessEmailEndpoint.cs** - Line ~17
   - **ProcessEmailHandler.cs** - Line ~11
   - **GenerateDraftEndpoint.cs** - Line ~15

### **Step 6: Test Your Breakpoints**
1. **Open your Insomnia collection**
2. **Send a "Process Email" request**
3. **VS Code should hit your breakpoint!** 🎉
4. **Use debug controls:**
   - **F10** - Step Over
   - **F11** - Step Into
   - **F5** - Continue
   - **Shift + F5** - Stop Debugging

## 🔧 **VS Code Debugging Features**

### **Debug Console:**
- **View** → **Debug Console** or `Ctrl + Shift + Y`
- Execute expressions while debugging:
  ```csharp
  command.EmailId
  existingEmail?.Subject
  analysis.Priority
  ```

### **Variables Panel:**
- Automatically shows all local variables
- Expand objects to inspect properties
- Right-click to "Add to Watch"

### **Watch Panel:**
- Add expressions to monitor:
  ```
  command.EmailId
  cancellationToken.IsCancellationRequested
  result.ProcessedEmailId
  ```

### **Call Stack:**
- See the execution path
- Click frames to navigate

## 🚨 **Troubleshooting VS Code Issues**

### **"No process found" Error:**
1. **Check container name:**
   ```powershell
   docker ps --format "table {{.Names}}\t{{.Status}}"
   ```
2. **Update launch.json with correct container name**
3. **Restart VS Code and try again**

### **Breakpoints Not Hitting:**
1. **Check Debug Output panel:**
   - **View** → **Output** → Select "C#" from dropdown
2. **Verify symbols loaded:**
   - Look for "Loaded symbols for..." messages
3. **Rebuild container if needed:**
   ```powershell
   docker-compose build emailintelligence.api
   docker-compose up -d emailintelligence.api
   ```

### **Source Mapping Issues:**
1. **Verify paths in launch.json match your workspace**
2. **Check sourceFileMap configuration**
3. **Ensure you opened the root workspace folder**

---

## **🔧 Advanced Troubleshooting & Tips**

### **Docker Debugging Issues:**

#### **"Cannot connect to Docker daemon" Error**
```powershell
# Check Docker status
docker version
docker ps

# If Docker Desktop isn't running, start it and wait for it to be ready
```

#### **"Process not found" when attaching**
1. **Verify container is running:**
   ```powershell
   docker ps | findstr emailintelligence
   ```
2. **Check exact container name:**
   ```powershell
   docker ps --format "table {{.Names}}\t{{.Image}}\t{{.Status}}"
   ```
3. **Update launch.json if container name differs**

#### **"vsdbg not found" Error**
The debugging tools should install automatically. If not:
```powershell
# Check if vsdbg exists in container
docker exec src-emailintelligence.api-1 ls -la /vsdbg

# If missing, rebuild the container
docker-compose build --no-cache emailintelligence.api
```

### **Performance & Advanced Features:**

#### **Hot Reload in Docker**
For faster development cycles, modify your `docker-compose.override.yml`:
```yaml
emailintelligence.api:
  environment:
    - DOTNET_WATCH_RESTART_ON_RUDE_EDIT=true
    - DOTNET_USE_POLLING_FILE_WATCHER=true
  volumes:
    - ./src/Services/EmailIntelligence:/src/Services/EmailIntelligence:cached
```

#### **Multiple Service Debugging**
Create compound configurations in `launch.json`:
```json
{
    "compounds": [
        {
            "name": "🔥 Debug All Services",
            "configurations": [
                "🐛 Attach to EmailIntelligence (Docker)",
                "🐛 Attach to Basket (Docker)"
            ]
        }
    ]
}
```

#### **Database Connection During Debugging**
Access the database while debugging:
```powershell
# Connect to EmailIntelligence database
docker exec -it src-emailintelligencedb-1 psql -U postgres -d EmailIntelligenceDb

# List tables
\dt

# Query email analytics
SELECT * FROM email_analytics LIMIT 10;
```

### **VS Code Extensions for Better Debugging:**

Install these extensions for enhanced debugging experience:
1. **C# for Visual Studio Code** (ms-dotnettools.csharp)
2. **C# Dev Kit** (ms-dotnettools.csdevkit)
3. **Docker** (ms-azuretools.vscode-docker)
4. **REST Client** (humao.rest-client) - Alternative to Insomnia
5. **GitLens** (eamodio.gitlens) - Git integration
6. **Thunder Client** (rangav.vscode-thunder-client) - API testing

### **Monitoring & Logging:**

#### **Real-time Log Monitoring**
```powershell
# Follow all services logs
docker-compose logs -f

# Follow specific service
docker-compose logs -f emailintelligence.api

# Filter logs by level
docker-compose logs -f emailintelligence.api | findstr "ERROR"
```

#### **Health Check Monitoring**
```powershell
# Check service health
curl http://localhost:6006/health

# Through YARP gateway
curl http://localhost:6000/emailintelligence-service/health
```

### **Quick Reference Commands:**

#### **Docker Management**
```powershell
# Start all services
docker-compose up -d

# Start specific service
docker-compose up -d emailintelligence.api

# Stop and remove containers
docker-compose down

# Rebuild specific service
docker-compose build emailintelligence.api

# View container logs
docker logs src-emailintelligence.api-1 --tail 50 -f

# Execute commands in container
docker exec -it src-emailintelligence.api-1 bash
```

#### **Debugging Workflow**
```powershell
# 1. Start services
docker-compose up -d

# 2. Check services are running
docker ps

# 3. Open VS Code in project root
code .

# 4. Start debugging (F5)
# 5. Set breakpoints
# 6. Test with Insomnia
# 7. Debug and fix issues
# 8. Hot reload will apply changes
```

## **🎯 Best Practices for VS Code Docker Debugging**

### **Project Organization:**
- **Keep the workspace root** at `c:\Repositories\run-aspnetcore-microservices`
- **Use relative paths** in configurations
- **Organize debug configurations** by service type

### **Debugging Strategy:**
1. **Start with Docker attach** for production-like debugging
2. **Use local debugging** for rapid development
3. **Set strategic breakpoints** in key business logic
4. **Use conditional breakpoints** for specific scenarios
5. **Monitor logs** alongside debugging

### **Code Quality:**
- **Use debug-specific code** sparingly
- **Leverage conditional compilation** (`#if DEBUG`)
- **Add meaningful logging** for production debugging
- **Write unit tests** to reduce debugging needs

---

Your VS Code setup is now production-ready for debugging the EmailIntelligence microservice! 🚀

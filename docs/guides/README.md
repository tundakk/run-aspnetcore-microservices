# 📖 Developer Guides

This section contains comprehensive guides for developers working with the EShop Microservices platform.

## 📋 Available Guides

### 🐛 [Debugging Guide](debugging-guide.md)
Complete guide for debugging the EmailIntelligence service and other microservices using Visual Studio Code and Docker.

### 🛠️ [Developer Guide](developer-guide.md)
Best practices and guidelines for developing with the EShop Microservices platform.

### 🚨 [Troubleshooting](troubleshooting.md)
Common issues and their solutions when working with the microservices platform.

### 🤝 [Contributing](contributing.md)
Guidelines for contributing to the project, including code standards and pull request process.

## 🎯 Quick Reference

### **Development Workflow**
1. **Setup Environment**: Follow the [setup guide](../setup/README.md)
2. **Start Services**: Use Docker Compose to run all services
3. **Set Breakpoints**: Configure debugging in VS Code
4. **Test Changes**: Use API collections for validation
5. **Commit Code**: Follow contribution guidelines

### **Key Development Tools**
- **VS Code**: Primary development environment
- **Docker**: Container runtime and orchestration
- **Insomnia/Postman**: API testing and validation
- **Git**: Version control and collaboration
- **PostgreSQL/SQL Server**: Database management

## 🏗️ Architecture Patterns

### **Clean Architecture**
```
📁 EmailIntelligence.API
├── 🎯 Presentation Layer (Controllers, Endpoints)
├── 🏢 Application Layer (Features, Handlers) 
├── 🏛️ Domain Layer (Entities, Value Objects)
└── 🗄️ Infrastructure Layer (Database, External APIs)
```

### **CQRS Pattern**
- **Commands**: Modify state (Create, Update, Delete)
- **Queries**: Read state (Get, List, Search)
- **Handlers**: Process commands and queries
- **Mediator**: Route requests to appropriate handlers

### **Event-Driven Architecture**
- **Publishers**: Services that emit events
- **Subscribers**: Services that consume events
- **Message Broker**: RabbitMQ for reliable messaging
- **Event Store**: Persistent event storage

## 🔧 Development Best Practices

### **Code Organization**
```
📁 src/Services/[ServiceName]/
├── 📁 [ServiceName].API/          # API endpoints and controllers
├── 📁 [ServiceName].Application/  # Business logic and handlers
├── 📁 [ServiceName].Domain/       # Domain entities and rules
└── 📁 [ServiceName].Infrastructure/ # Data access and external services
```

### **Naming Conventions**
- **Classes**: PascalCase (`EmailProcessor`)
- **Methods**: PascalCase (`ProcessEmail`)
- **Variables**: camelCase (`emailData`)
- **Constants**: UPPER_CASE (`MAX_RETRY_COUNT`)
- **Files**: PascalCase matching class name

### **Error Handling**
```csharp
// Use result patterns for operation outcomes
public async Task<Result<ProcessedEmail>> ProcessEmailAsync(ProcessEmailCommand command)
{
    try
    {
        // Business logic here
        return Result.Success(processedEmail);
    }
    catch (ValidationException ex)
    {
        return Result.Failure<ProcessedEmail>(ex.Message);
    }
}
```

## 🧪 Testing Strategy

### **Test Pyramid**
```
       🔺 E2E Tests (Few)
      🔺🔺 Integration Tests (Some)  
    🔺🔺🔺 Unit Tests (Many)
```

### **Testing Types**
- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test service interactions
- **E2E Tests**: Test complete user workflows
- **Contract Tests**: Verify API contracts between services

### **Test Organization**
```
📁 tests/
├── 📁 UnitTests/
│   ├── 📁 EmailIntelligence.Domain.Tests/
│   ├── 📁 EmailIntelligence.Application.Tests/
│   └── 📁 EmailIntelligence.Infrastructure.Tests/
├── 📁 IntegrationTests/
│   └── 📁 EmailIntelligence.API.IntegrationTests/
└── 📁 E2ETests/
    └── 📁 EShop.E2ETests/
```

## 🔍 Code Quality

### **Static Analysis**
```xml
<!-- Directory.Build.props -->
<PropertyGroup>
  <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  <WarningsAsErrors />
  <WarningsNotAsErrors>CS1591</WarningsNotAsErrors>
</PropertyGroup>
```

### **Code Formatting**
```json
// .editorconfig
[*.cs]
indent_style = space
indent_size = 4
end_of_line = crlf
trim_trailing_whitespace = true
insert_final_newline = true
```

### **Code Analysis Rules**
- Use nullable reference types
- Implement async/await properly
- Follow SOLID principles
- Write self-documenting code
- Add XML documentation for public APIs

## 📊 Performance Guidelines

### **Database Optimization**
- Use proper indexing strategies
- Implement connection pooling
- Use async operations for I/O
- Consider read replicas for heavy queries
- Implement database migrations

### **API Performance**
- Use appropriate HTTP status codes
- Implement response caching
- Use compression for large responses
- Implement rate limiting
- Monitor response times

### **Memory Management**
- Dispose of resources properly
- Use object pooling for frequent allocations
- Monitor garbage collection
- Use memory profiling tools
- Implement circuit breakers

## 🔐 Security Guidelines

### **Authentication & Authorization**
```csharp
[Authorize(Roles = "Admin")]
[HttpPost("sensitive-operation")]
public async Task<IActionResult> SensitiveOperation()
{
    // Secure operation
}
```

### **Input Validation**
```csharp
public class ProcessEmailCommand
{
    [Required]
    [EmailAddress]
    public string From { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Subject { get; set; }
}
```

### **Data Protection**
- Encrypt sensitive data at rest
- Use HTTPS for all communications
- Implement proper logging (no sensitive data)
- Use secure configuration management
- Regular security audits

## 🚀 Deployment Practices

### **Configuration Management**
```csharp
// Use strongly-typed configuration
public class LLMSettings
{
    public string ApiKey { get; set; }
    public string BaseUrl { get; set; }
    public string Model { get; set; }
}

// Register in Program.cs
services.Configure<LLMSettings>(configuration.GetSection("LLMSettings"));
```

### **Health Checks**
```csharp
services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRabbitMQ(rabbitConnectionString)
    .AddUrlGroup(new Uri("https://api.openai.com"), "openai");
```

### **Logging Configuration**
```csharp
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddApplicationInsights();
    builder.SetMinimumLevel(LogLevel.Information);
});
```

## 📈 Monitoring & Observability

### **Structured Logging**
```csharp
_logger.LogInformation("Processing email {EmailId} for user {UserId}", 
    command.EmailId, command.UserId);
```

### **Metrics Collection**
- Request duration and frequency
- Error rates and types
- Database query performance
- External API response times
- Business metrics (emails processed, etc.)

### **Distributed Tracing**
- Use correlation IDs for request tracking
- Implement OpenTelemetry for distributed tracing
- Monitor cross-service communication
- Track performance bottlenecks

## 🛠️ Development Tools Setup

### **Required Tools**
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### **Recommended Extensions**
```json
{
  "recommendations": [
    "ms-dotnettools.csharp",
    "ms-dotnettools.csdevkit",
    "ms-azuretools.vscode-docker",
    "humao.rest-client",
    "eamodio.gitlens",
    "bradlc.vscode-tailwindcss"
  ]
}
```

### **Development Environment**
```powershell
# Install .NET tools
dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator

# Verify installation
dotnet --version
docker --version
git --version
```

## 🤝 Collaboration

### **Git Workflow**
1. Create feature branch: `git checkout -b feature/email-analytics`
2. Make changes and commit: `git commit -m "Add email analytics feature"`
3. Push branch: `git push origin feature/email-analytics`
4. Create pull request with description
5. Code review and approval
6. Merge to main branch

### **Communication**
- Use descriptive commit messages
- Write clear pull request descriptions
- Add comments for complex business logic
- Update documentation with changes
- Participate in code reviews

## 📚 Learning Resources

### **Architecture Patterns**
- [Clean Architecture by Robert Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Event-Driven Architecture](https://microservices.io/patterns/data/event-driven-architecture.html)

### **Technology Documentation**
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)

Happy coding! 🚀

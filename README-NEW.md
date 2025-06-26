# 🚀 EShop Microservices Platform

**UDEMY COURSE WITH DISCOUNTED - Step by Step Development of this Repository -> https://www.udemy.com/course/microservices-architecture-and-implementation-on-dotnet/?couponCode=MAYY25**

See the overall picture of **implementations on microservices with .net tools** on real-world **e-commerce microservices** project;

![microservices](https://github.com/aspnetrun/run-aspnetcore-microservices/assets/1147445/efe5e688-67f2-4ddd-af37-d9d3658aede4)

A comprehensive e-commerce platform built with **ASP.NET Core microservices**, featuring **Catalog, Basket, Discount, Ordering**, and **EmailIntelligence** services. The platform uses **NoSQL (DocumentDb, Redis)** and **Relational databases (PostgreSQL, SQL Server)** with **RabbitMQ Event Driven Communication** and **YARP API Gateway**.

## 📚 **Documentation**

> **📖 [Complete Documentation](docs/README.md)** - All setup guides, architecture docs, and developer resources

### 🎯 **Quick Links**
- **🚀 [Getting Started](docs/setup/README.md)** - Setup your development environment
- **🏗️ [Architecture Overview](docs/architecture/README.md)** - System design and patterns
- **🧪 [Testing Guide](docs/testing/testing-guide.md)** - API testing and validation
- **🐛 [Debugging Guide](docs/guides/debugging-guide.md)** - VS Code debugging setup
- **☁️ [Deployment](docs/deployment/README.md)** - Docker and Azure deployment

## ⚡ **Quick Start**

### **1. Prerequisites**
- [Docker Desktop](https://www.docker.com/products/docker-desktop) running
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) installed
- [OpenAI API Key](https://platform.openai.com/api-keys) for EmailIntelligence service

### **2. Clone & Start**
```bash
git clone https://github.com/aspnetrun/run-aspnetcore-microservices.git
cd run-aspnetcore-microservices

# Set your OpenAI API key
export LLMSettings__ApiKey="your-openai-api-key"

# Start all services
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

### **3. Access Services**
- **🌐 Web Application**: http://localhost:6005
- **🚪 API Gateway**: http://localhost:6004
- **🧠 EmailIntelligence API**: http://localhost:6006
- **📨 RabbitMQ Management**: http://localhost:15672 (guest/guest)

## 🏛️ **System Architecture**

### **Microservices**
| Service | Port | Description | Technology |
|---------|------|-------------|------------|
| **🛍️ Catalog** | 6000 | Product catalog management | Minimal APIs, PostgreSQL, Marten |
| **🧺 Basket** | 6001 | Shopping cart functionality | Web API, Redis, PostgreSQL |
| **💰 Discount** | 6002 | Pricing and discounts | gRPC, SQLite, EF Core |
| **📦 Ordering** | 6003 | Order processing | DDD/CQRS, SQL Server, MediatR |
| **🧠 EmailIntelligence** | 6006 | AI-powered email processing | Clean Architecture, OpenAI API |
| **🌐 API Gateway** | 6004 | Request routing | YARP Reverse Proxy |
| **🖥️ Web UI** | 6005 | User interface | Blazor Server |

### **Infrastructure**
- **📊 Databases**: PostgreSQL, SQL Server, Redis, SQLite
- **📨 Message Broker**: RabbitMQ with management UI
- **🤖 AI Integration**: OpenAI GPT for email intelligence
- **🐳 Containerization**: Docker Compose orchestration

## ✨ **Key Features**

### **🛍️ Catalog Service**
- ASP.NET Core **Minimal APIs** with latest .NET 8 features
- **Vertical Slice Architecture** with feature folders
- **CQRS** implementation using MediatR library
- **Marten** library for .NET Transactional Document DB on PostgreSQL
- **Carter** for minimal API endpoint definition

### **🧺 Basket Service**
- ASP.NET 8 **Web API** following REST principles
- **Redis** distributed cache with cache-aside pattern
- **gRPC** communication with Discount service
- **RabbitMQ** event publishing for basket checkout

### **💰 Discount Service**
- High-performance **gRPC Server** application
- **Protocol Buffers** for efficient serialization
- **Entity Framework Core** with SQLite
- Inter-service communication optimization

### **📦 Ordering Service**
- **Domain-Driven Design (DDD)** implementation
- **CQRS** with MediatR, FluentValidation, and Mapster
- **Event-driven** architecture with RabbitMQ
- **SQL Server** with Entity Framework Core

### **🧠 EmailIntelligence Service**
- **AI-powered** email analysis and response generation
- **OpenAI GPT integration** for natural language processing
- **Clean Architecture** with CQRS patterns
- **PostgreSQL** for persistent storage
- **Real-time** email categorization and priority assessment

### **🌐 API Gateway & Web UI**
- **YARP** reverse proxy for API aggregation
- **Rate limiting** and request routing
- **Blazor** web application with Bootstrap UI
- **Refit** HTTP client for service communication

## 🎯 **Business Capabilities**

### **E-Commerce Flow**
1. **Browse Products** → Catalog service provides product information
2. **Add to Basket** → Basket service manages shopping cart with Redis caching
3. **Apply Discounts** → Discount service calculates pricing via gRPC
4. **Place Order** → Ordering service processes with DDD patterns
5. **Email Notifications** → EmailIntelligence generates AI-powered communications

### **AI-Powered Features**
- **📧 Email Categorization**: Automatic classification (urgent, informational, etc.)
- **🎯 Priority Assessment**: AI-driven priority scoring
- **✍️ Response Generation**: Context-aware email draft creation
- **📊 Sentiment Analysis**: Email tone and sentiment evaluation
- **🏷️ Keyword Extraction**: Automatic tag generation

## 🛠️ **Technology Stack**

### **Backend Technologies**
- **.NET 8** - Latest framework with C# 12
- **ASP.NET Core** - Web APIs and minimal APIs
- **Entity Framework Core** - ORM with migrations
- **MediatR** - CQRS and mediator patterns
- **FluentValidation** - Request validation
- **Mapster** - Object mapping

### **Databases & Storage**
- **PostgreSQL** - Primary relational database
- **SQL Server** - Complex transactional data
- **Redis** - Distributed caching and session storage
- **SQLite** - Lightweight embedded database

### **Communication**
- **gRPC** - High-performance inter-service communication
- **RabbitMQ** - Asynchronous message broker
- **HTTP/REST** - Standard web API communication
- **YARP** - API gateway and reverse proxy

### **External Integrations**
- **OpenAI API** - GPT models for AI capabilities
- **Docker** - Containerization and orchestration

## 🧪 **Testing & Development**

### **API Testing**
- **📧 [Insomnia Collection](docs/collections/EmailIntelligence-Insomnia-Collection.json)** - EmailIntelligence API testing
- **🛍️ [Postman Collection](docs/collections/EShopMicroservices.postman_collection.json)** - Full platform testing
- **🔍 Health Check Endpoints** - Service monitoring and validation

### **Development Tools**
- **🐛 VS Code Debugging** - Docker container debugging support
- **📊 Database Seeding** - Comprehensive test data
- **📋 API Documentation** - Interactive Swagger/OpenAPI docs
- **🔄 Hot Reload** - Fast development iteration

## 📖 **Learning Resources**

### **Architecture Patterns**
- [.NET 8 Microservices: DDD, CQRS, Vertical/Clean Architecture](https://medium.com/@mehmetozkaya/net-8-microservices-ddd-cqrs-vertical-clean-architecture-2dd7ebaaf4bd)
- **Event-Driven Communication** patterns
- **Clean Architecture** implementation
- **CQRS** with MediatR patterns

### **Course & Tutorials**
- **[Udemy Course](https://www.udemy.com/course/microservices-architecture-and-implementation-on-dotnet/?couponCode=MAYY25)** - Step-by-step development
- **Documentation** - Comprehensive guides and tutorials
- **Code Examples** - Real-world implementation patterns

## 🚀 **Getting Started**

1. **📚 [Read the Documentation](docs/README.md)** - Complete setup and architecture guide
2. **🛠️ [Follow Setup Guide](docs/setup/README.md)** - Step-by-step environment setup
3. **🧪 [Test the APIs](docs/testing/testing-guide.md)** - Validate your installation
4. **🐛 [Setup Debugging](docs/guides/debugging-guide.md)** - Configure VS Code debugging
5. **☁️ [Deploy to Cloud](docs/deployment/azure-deployment.md)** - Production deployment

## 👥 **Authors & Contributors**

* **Mehmet Ozkaya** - *Initial work* - [mehmetozkaya](https://github.com/mehmetozkaya)

See the list of [contributors](https://github.com/aspnetrun/run-core/contributors) who participated in this project.

## 📞 **Support**

- **🐛 Issues**: [GitHub Issues](https://github.com/aspnetrun/run-aspnetcore-microservices/issues)
- **📖 Documentation**: [Project Documentation](docs/README.md)
- **💬 Discussions**: [GitHub Discussions](https://github.com/aspnetrun/run-aspnetcore-microservices/discussions)

---

**🎯 Build modern, scalable e-commerce platforms with .NET 8 microservices!**

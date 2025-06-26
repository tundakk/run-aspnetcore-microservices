# 📚 EShop Microservices Documentation

Welcome to the comprehensive documentation for the EShop Microservices project - a modern e-commerce platform built with .NET 8 microservices architecture.

## 🎯 Quick Navigation

### 🚀 Getting Started
- [**Project Overview**](../README.md) - Main project documentation
- [**Setup Guide**](setup/README.md) - Complete setup instructions
- [**API Key Configuration**](setup/api-key-setup.md) - Security configuration

### 🏗️ Architecture & Design
- [**Architecture Diagram**](architecture/EmailIntelligence-Architecture-Diagram.html) - Visual system overview
- [**Microservices Overview**](architecture/microservices-overview.md) - Service breakdown and responsibilities

### 📋 Setup & Configuration
- [**EmailIntelligence Setup**](setup/emailintelligence-setup.md) - Detailed EmailIntelligence service setup
- [**Debugging Guide**](guides/debugging-guide.md) - Docker debugging procedures
- [**Seed Data Documentation**](setup/seed-data-documentation.md) - Database initialization

### 🧪 Testing & API
- [**Testing Guide**](testing/testing-guide.md) - Comprehensive testing procedures
- [**API Collections**](collections/README.md) - Postman and Insomnia collections
- [**Test Data Samples**](testing/test-data.md) - Sample JSON data for testing

### 🚀 Deployment
- [**Azure Deployment**](deployment/azure-deployment.md) - Cloud deployment guide
- [**Docker Deployment**](deployment/docker-deployment.md) - Container deployment
- [**Production Configuration**](deployment/production-config.md) - Production setup

### 📖 User Guides
- [**Developer Guide**](guides/developer-guide.md) - Development best practices
- [**Troubleshooting**](guides/troubleshooting.md) - Common issues and solutions
- [**Contributing**](guides/contributing.md) - How to contribute to the project

## 🏛️ Project Architecture

This project implements a modern microservices architecture with the following services:

### Core Services
- **🛍️ Catalog Service** - Product catalog management with Minimal APIs
- **🧺 Basket Service** - Shopping cart functionality with Redis caching
- **💰 Discount Service** - Pricing and discount management via gRPC
- **📦 Ordering Service** - Order processing with DDD/CQRS patterns
- **🧠 EmailIntelligence Service** - AI-powered email processing

### Infrastructure
- **🌐 API Gateway** - YARP-based API routing and aggregation
- **🕸️ Web Application** - Blazor-based shopping interface
- **📨 Message Broker** - RabbitMQ for async communication
- **💾 Databases** - PostgreSQL, SQL Server, Redis, SQLite

## 🛠️ Technology Stack

- **.NET 8** - Latest .NET framework
- **Docker** - Containerization
- **PostgreSQL/SQL Server** - Relational databases
- **Redis** - Distributed caching
- **RabbitMQ** - Message queuing
- **gRPC** - High-performance RPC
- **MediatR** - CQRS implementation
- **Carter** - Minimal API framework
- **OpenAI API** - AI capabilities

## 📊 Key Features

- ✅ **Microservices Architecture** with proper service boundaries
- ✅ **Event-Driven Communication** using RabbitMQ
- ✅ **CQRS & DDD** patterns implementation
- ✅ **API Gateway** with YARP
- ✅ **Distributed Caching** with Redis
- ✅ **Container Orchestration** with Docker Compose
- ✅ **AI Integration** with OpenAI
- ✅ **Clean Architecture** and best practices

## 🔗 External Resources

- [Udemy Course](https://www.udemy.com/course/microservices-architecture-and-implementation-on-dotnet/?couponCode=MAYY25) - Step-by-step development course
- [Medium Article](https://medium.com/@mehmetozkaya/net-8-microservices-ddd-cqrs-vertical-clean-architecture-2dd7ebaaf4bd) - Detailed explanation

## 📞 Support

If you encounter any issues or have questions:

1. Check the [**Troubleshooting Guide**](guides/troubleshooting.md)
2. Review the [**Setup Documentation**](setup/README.md)
3. Consult the [**Testing Guide**](testing/testing-guide.md)
4. Open an issue in the repository

---

**Happy Coding! 🚀**

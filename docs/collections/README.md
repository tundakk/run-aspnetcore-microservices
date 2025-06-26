# API Collections

This directory contains API testing collections for all microservices in the EShop project.

## Collections Available

### 📧 EmailIntelligence-Insomnia-Collection.json
Comprehensive Insomnia collection for testing the EmailIntelligence microservice:

**Features:**
- ✅ Complete workflow testing (email processing → draft generation)
- ✅ All endpoint coverage (direct API + YARP gateway)
- ✅ Environment variables pre-configured
- ✅ Sample test data included
- ✅ Error scenario testing
- ✅ Health check endpoints

**Endpoints Included:**
- Email Processing (`POST /emails/process`)
- Draft Generation (`POST /drafts/generate`)  
- Email Filtering (`GET /emails/filtered`)
- Health Checks (`GET /health`)
- YARP Gateway variants for all endpoints

### 🛍️ EShopMicroservices.postman_collection.json
Postman collection covering all microservices in the e-commerce platform:

**Services Covered:**
- Catalog Service
- Basket Service  
- Discount Service
- Ordering Service
- EmailIntelligence Service
- API Gateway (YARP)

## How to Use

### Insomnia Setup
1. **Open Insomnia**
2. **Import Collection**: File → Import Data → Select `EmailIntelligence-Insomnia-Collection.json`
3. **Configure Environment**: 
   - `base_url`: `http://localhost:6006` (direct API)
   - `yarp_url`: `http://localhost:6004` (API gateway)
4. **Start Testing**: Begin with health checks, then follow the workflow guide

### Postman Setup  
1. **Open Postman**
2. **Import Collection**: File → Import → Select `EShopMicroservices.postman_collection.json`
3. **Set Environment Variables**:
   - Create environment with service URLs
   - Configure any authentication tokens if required
4. **Run Collection**: Use collection runner for automated testing

## Testing Workflow

### EmailIntelligence Service Testing
1. **Health Check** → Verify service is running
2. **Process Email** → Submit email for AI analysis  
3. **Generate Draft** → Create AI-powered response draft
4. **Validate Results** → Check confidence scores and content quality

### Full Microservices Testing
1. **Start Services**: `docker-compose up -d`
2. **Health Checks**: Verify all services are healthy
3. **End-to-End Workflow**:
   - Browse catalog
   - Add items to basket
   - Apply discounts  
   - Place order
   - Process order emails with EmailIntelligence

## Environment Configuration

### Local Development
```
base_url = http://localhost:6006
yarp_url = http://localhost:6004
catalog_url = http://localhost:6000
basket_url = http://localhost:6001
discount_url = http://localhost:6002
ordering_url = http://localhost:6003
gateway_url = http://localhost:6004
web_url = http://localhost:6005
```

### Docker Environment
All services accessible through their published ports as configured in `docker-compose.override.yml`.

## Troubleshooting

### Common Issues
- **Connection Refused**: Verify services are running with `docker ps`
- **Invalid Responses**: Check service logs with `docker-compose logs -f [service-name]`
- **Environment Variables**: Ensure correct URLs and ports are configured

### Debug Steps
1. Verify Docker containers are running
2. Check service health endpoints
3. Review service logs for errors
4. Validate request format and required fields
5. Test direct service access before gateway access

## Contributing

When adding new endpoints or services:
1. Update the appropriate collection file
2. Add sample request/response examples
3. Include error scenario testing
4. Update environment variable documentation
5. Test collection thoroughly before committing

---

These collections provide comprehensive testing capabilities for both individual services and full system integration scenarios.

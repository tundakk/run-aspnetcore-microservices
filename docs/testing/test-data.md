# Test Data Samples

This directory contains sample test data for the EmailIntelligence service API testing.

## Files

### test-email.json
Sample email data for testing the email processing endpoint. Contains various email scenarios including urgent messages, informational emails, and action-required communications.

### test-draft.json  
Sample draft generation requests for testing the AI-powered draft generation endpoint. Includes different context scenarios and user instructions.

## Usage

Use these sample JSON files with API testing tools like:
- Insomnia
- Postman  
- curl commands
- Automated test suites

## Email Processing Test Examples

### Basic Email Processing
```json
{
  "EmailId": "test-001",
  "UserId": "user-123",
  "Subject": "Project Status Update",
  "From": "project.manager@company.com",
  "To": "team@company.com",
  "Body": "Hello team, I wanted to provide you with an update on our current project status...",
  "ReceivedAt": "2025-06-26T10:00:00Z"
}
```

### Urgent Email Processing
```json
{
  "EmailId": "urgent-001", 
  "UserId": "user-123",
  "Subject": "URGENT: Server Performance Issues",
  "From": "admin@company.com",
  "To": "devops@company.com",
  "Body": "We are experiencing critical server performance issues that need immediate attention...",
  "ReceivedAt": "2025-06-26T14:30:00Z"
}
```

## Draft Generation Test Examples

### Basic Draft Generation
```json
{
  "ProcessedEmailId": "{{processedEmailId}}",
  "UserId": "user-123",
  "AdditionalContext": "Please keep response professional and acknowledge receipt"
}
```

### Draft with Custom Context
```json
{
  "ProcessedEmailId": "{{processedEmailId}}",
  "UserId": "user-123", 
  "AdditionalContext": "Schedule meeting for next week and include budget considerations"
}
```

## Testing Workflow

1. **Process Email**: Use email JSON to process and analyze an email
2. **Get ProcessedEmailId**: Extract the ID from the response
3. **Generate Draft**: Use the ProcessedEmailId to generate a response draft
4. **Validate Results**: Check confidence scores and generated content quality

## Validation Requirements

### Email Processing
- EmailId: Required string
- UserId: Required string  
- Subject: Required string
- From: Required valid email format
- To: Required valid email format
- Body: Required string
- ReceivedAt: Required DateTime in ISO format

### Draft Generation
- ProcessedEmailId: Required GUID from previous email processing
- UserId: Required string matching original email processor
- AdditionalContext: Optional string for custom instructions

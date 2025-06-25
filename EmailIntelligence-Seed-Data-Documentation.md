# EmailIntelligence Seed Data Documentation

## Overview
This document describes the comprehensive seed data added to the EmailIntelligence service, providing sample emails for all categories and realistic test data for development and testing purposes.

## Seed Data Components

### 1. Processed Emails (13 samples)
The service includes sample emails covering all 10 categories defined in the `EmailCategory` enum:

#### Categories Covered:
1. **RequiresResponse** (2 emails)
   - Project Budget Approval
   - Contract Review Request

2. **Informational** (2 emails)
   - Company Policy Update
   - System Outage Report

3. **ActionRequired** (1 email)
   - Security Certificate Renewal (URGENT)

4. **Meeting** (2 emails)
   - Weekly Team Standup
   - Quarterly All-Hands Meeting

5. **Support** (1 email)
   - Login Issue Resolution

6. **Marketing** (1 email)
   - Promotional Discount Offer

7. **Newsletter** (1 email)
   - Tech Weekly Newsletter

8. **Spam** (1 email)
   - Lottery Scam Email

9. **Personal** (1 email)
   - Family Reunion Planning

10. **Internal** (1 email)
    - Server Maintenance Notice

#### Email Attributes:
- **Realistic content** with appropriate tone and context
- **Priority levels** from Low to Critical
- **Confidence scores** ranging from 0.82 to 0.98
- **Keywords extraction** for each email
- **Action items** for emails requiring response
- **Varied timestamps** spanning 30 days

### 2. User Tone Profiles (3 samples)
Sample user profiles representing different communication styles:

1. **Professional User** (`user123@example.com`)
   - Style: Professional, concise, friendly
   - Confidence: 85% (after 15 emails sampled)
   - Preferred phrases: Collaborative language
   - Avoided phrases: Urgent terminology

2. **Manager** (`manager@company.com`)
   - Style: Direct, authoritative, results-oriented
   - Confidence: 92% (after 25 emails sampled)
   - Preferred phrases: Action-oriented language
   - Avoided phrases: Uncertain language

3. **Developer** (`developer@company.com`)
   - Style: Technical, detailed, collaborative
   - Confidence: 78% (after 12 emails sampled)
   - Preferred phrases: Casual but informative
   - Avoided phrases: Formal corporate language

### 3. Email Drafts (4 samples)
Sample AI-generated draft responses with different statuses:

1. **Budget Approval Response** (UserEdited + Approved)
   - Original AI generation with professional tone
   - User editing for brevity and tone adjustment
   - Final approval for sending

2. **Contract Review Response** (Generated)
   - Fresh AI-generated draft
   - Awaiting user review

3. **Family Reunion Response** (UserEdited + Approved)
   - Personal tone with emoji additions
   - User personalization and enthusiasm enhancement
   - Approved for sending

4. **Security Certificate Acknowledgment** (Sent)
   - Urgent response with action plan
   - Marked as sent (completed workflow)

## Database Schema Impact

### Tables Populated:
- `processed_emails` - 13 records
- `user_tone_profiles` - 3 records  
- `email_drafts` - 4 records

### Relationships:
- Email drafts reference their corresponding processed emails
- All data uses consistent user IDs for relationship integrity

## Usage Benefits

### For Development:
- Complete dataset for testing all email categories
- Realistic content for UI/UX testing
- Proper foreign key relationships for testing queries

### For AI Training:
- Diverse email content for machine learning validation
- User feedback examples (corrections, edits)
- Tone analysis samples across different user types

### For Demo Purposes:
- Professional-looking sample data
- Covers all major email use cases
- Shows complete user workflow from analysis to draft generation

## Implementation Details

### Seeding Process:
1. **Automatic seeding** on application startup
2. **Conditional seeding** - only runs if tables are empty
3. **Logging** for monitoring seed data application
4. **Error handling** with comprehensive logging

### Files Modified:
- `EmailIntelligenceDbContextSeed.cs` - Main seed data logic
- `HostExtensions.cs` - Database initialization helper
- `Program.cs` - Startup integration

### Configuration:
- Seeds run automatically during `app.InitializeDatabaseAsync()`
- No manual intervention required
- Safe for production (only runs when database is empty)

## Testing the Seed Data

### Verification Queries:
```sql
-- Check email distribution by category
SELECT category, COUNT(*) as count 
FROM processed_emails 
GROUP BY category;

-- Check user tone profiles
SELECT user_id, writing_style, confidence_level 
FROM user_tone_profiles;

-- Check draft statuses
SELECT status, COUNT(*) as count 
FROM email_drafts 
GROUP BY status;
```

### API Endpoints to Test:
- GET `/api/emails` - View processed emails
- GET `/api/emails/{id}` - Individual email details
- GET `/api/drafts` - Generated drafts
- GET `/api/users/{id}/tone-profile` - User tone analysis

The seed data provides a comprehensive foundation for developing, testing, and demonstrating the EmailIntelligence service capabilities across all supported email categories and user workflows.

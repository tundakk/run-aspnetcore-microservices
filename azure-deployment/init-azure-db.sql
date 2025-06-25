-- Azure PostgreSQL initialization script for EmailIntelligence
-- This script creates the necessary tables and initial data

-- Create the database schema for EmailIntelligence
-- Note: The database itself is created via Bicep template

-- Create tables (these will be created by EF Core migrations, but we include them for reference)

-- ProcessedEmails table
CREATE TABLE IF NOT EXISTS "ProcessedEmails" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" text NOT NULL,
    "OriginalEmail" text NOT NULL,
    "ProcessedContent" text NOT NULL,
    "ToneProfile" text NOT NULL,
    "ProcessedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
    "Metadata" jsonb,
    CONSTRAINT "PK_ProcessedEmails" PRIMARY KEY ("Id")
);

-- EmailDrafts table
CREATE TABLE IF NOT EXISTS "EmailDrafts" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" text NOT NULL,
    "Subject" text NOT NULL,
    "Content" text NOT NULL,
    "ToneStyle" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
    "ModifiedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
    "IsFinalized" boolean NOT NULL DEFAULT false,
    "Metadata" jsonb,
    CONSTRAINT "PK_EmailDrafts" PRIMARY KEY ("Id")
);

-- UserToneProfiles table
CREATE TABLE IF NOT EXISTS "UserToneProfiles" (
    "Id" uuid NOT NULL DEFAULT gen_random_uuid(),
    "UserId" text NOT NULL,
    "ProfileName" text NOT NULL,
    "ToneSettings" jsonb NOT NULL,
    "IsDefault" boolean NOT NULL DEFAULT false,
    "CreatedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
    "UpdatedDate" timestamp with time zone NOT NULL DEFAULT NOW(),
    CONSTRAINT "PK_UserToneProfiles" PRIMARY KEY ("Id")
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS "IX_ProcessedEmails_UserId" ON "ProcessedEmails" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_ProcessedEmails_ProcessedDate" ON "ProcessedEmails" ("ProcessedDate");
CREATE INDEX IF NOT EXISTS "IX_EmailDrafts_UserId" ON "EmailDrafts" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_EmailDrafts_CreatedDate" ON "EmailDrafts" ("CreatedDate");
CREATE INDEX IF NOT EXISTS "IX_UserToneProfiles_UserId" ON "UserToneProfiles" ("UserId");

-- Insert sample data for testing
INSERT INTO "UserToneProfiles" ("Id", "UserId", "ProfileName", "ToneSettings", "IsDefault", "CreatedDate", "UpdatedDate")
VALUES 
    (gen_random_uuid(), 'azure-test-user', 'Professional', '{"formality": "high", "friendliness": "medium", "assertiveness": "medium"}', true, NOW(), NOW()),
    (gen_random_uuid(), 'azure-test-user', 'Casual', '{"formality": "low", "friendliness": "high", "assertiveness": "low"}', false, NOW(), NOW()),
    (gen_random_uuid(), 'azure-test-user', 'Assertive', '{"formality": "medium", "friendliness": "low", "assertiveness": "high"}', false, NOW(), NOW())
ON CONFLICT DO NOTHING;

INSERT INTO "EmailDrafts" ("Id", "UserId", "Subject", "Content", "ToneStyle", "CreatedDate", "ModifiedDate", "IsFinalized")
VALUES 
    (gen_random_uuid(), 'azure-test-user', 'Azure Deployment Test', 'This is a test email draft created during Azure deployment.', 'Professional', NOW(), NOW(), false),
    (gen_random_uuid(), 'azure-test-user', 'Welcome Message', 'Welcome to the EmailIntelligence service! This service helps you analyze and improve your email communication.', 'Casual', NOW(), NOW(), true)
ON CONFLICT DO NOTHING;

INSERT INTO "ProcessedEmails" ("Id", "UserId", "OriginalEmail", "ProcessedContent", "ToneProfile", "ProcessedDate")
VALUES 
    (gen_random_uuid(), 'azure-test-user', 'Hello, I hope this email finds you well.', 'Hello, I hope this email finds you well. [Tone: Professional, Sentiment: Positive]', 'Professional', NOW()),
    (gen_random_uuid(), 'azure-test-user', 'Thanks for your help!', 'Thanks for your help! [Tone: Casual, Sentiment: Grateful]', 'Casual', NOW())
ON CONFLICT DO NOTHING;

-- Grant necessary permissions (if using specific database user)
-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO emailintelligence_user;
-- GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO emailintelligence_user;

-- Display success message
SELECT 'EmailIntelligence database initialized successfully!' as message;

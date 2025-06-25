-- Create EmailIntelligence Database Schema and Test Data

-- Create tables
CREATE TABLE IF NOT EXISTS processed_emails (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email_id VARCHAR(255) NOT NULL,
    user_id VARCHAR(255) NOT NULL,
    subject VARCHAR(500) NOT NULL,
    sender VARCHAR(255) NOT NULL,
    recipients TEXT NOT NULL,
    body TEXT NOT NULL,
    received_at TIMESTAMP NOT NULL,
    priority INTEGER NOT NULL DEFAULT 1,
    category INTEGER NOT NULL DEFAULT 1,
    requires_response BOOLEAN NOT NULL DEFAULT false,
    confidence_score DOUBLE PRECISION NOT NULL DEFAULT 0.0,
    keywords TEXT[],
    action_items TEXT,
    processed_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS email_drafts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    processed_email_id UUID NOT NULL,
    user_id VARCHAR(255) NOT NULL,
    generated_content TEXT NOT NULL,
    edited_content TEXT,
    confidence_score DOUBLE PRECISION NOT NULL DEFAULT 0.0,
    status INTEGER NOT NULL DEFAULT 0,
    additional_context TEXT,
    edit_types TEXT[],
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (processed_email_id) REFERENCES processed_emails(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS user_tone_profiles (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id VARCHAR(255) NOT NULL UNIQUE,
    tone_characteristics JSONB NOT NULL,
    example_texts TEXT[],
    confidence_level DOUBLE PRECISION NOT NULL DEFAULT 0.0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_processed_emails_user_id ON processed_emails(user_id);
CREATE INDEX IF NOT EXISTS idx_processed_emails_priority ON processed_emails(priority);
CREATE INDEX IF NOT EXISTS idx_processed_emails_category ON processed_emails(category);
CREATE INDEX IF NOT EXISTS idx_processed_emails_received_at ON processed_emails(received_at);
CREATE INDEX IF NOT EXISTS idx_email_drafts_user_id ON email_drafts(user_id);
CREATE INDEX IF NOT EXISTS idx_email_drafts_processed_email_id ON email_drafts(processed_email_id);
CREATE INDEX IF NOT EXISTS idx_user_tone_profiles_user_id ON user_tone_profiles(user_id);

-- Insert test data
INSERT INTO processed_emails (
    email_id, user_id, subject, sender, recipients, body, received_at, 
    priority, category, requires_response, confidence_score, keywords, action_items
) VALUES 
-- High Priority Business Emails
('email-001', 'user-john', 'Urgent: Server Performance Issues', 'ops-team@company.com', 'john.doe@company.com', 
 'Hi John, we are experiencing critical server performance issues on the production environment. The response time has increased by 300% in the last hour. Please investigate immediately and provide an update within 30 minutes.', 
 CURRENT_TIMESTAMP - INTERVAL '2 hours', 3, 3, true, 0.95, 
 ARRAY['urgent', 'server', 'performance', 'production', 'critical'], 
 'Investigate server performance, provide status update within 30 minutes'),

('email-002', 'user-john', 'Q4 Budget Review Meeting Tomorrow', 'finance@company.com', 'john.doe@company.com,team-leads@company.com', 
 'Dear Team, this is a reminder that we have the Q4 budget review meeting scheduled for tomorrow at 2 PM in Conference Room A. Please bring your department budget proposals and Q3 expense reports. The meeting agenda is attached.', 
 CURRENT_TIMESTAMP - INTERVAL '1 day', 2, 4, true, 0.88, 
 ARRAY['meeting', 'budget', 'Q4', 'finance', 'reminder'], 
 'Prepare budget proposals, bring Q3 expense reports, attend meeting at 2 PM'),

('email-003', 'user-john', 'Customer Complaint - Priority Response Needed', 'support@company.com', 'john.doe@company.com', 
 'John, we received a complaint from Premium Customer ABC Corp about delayed delivery of their order #12345. They are threatening to cancel their annual contract worth $500K. This needs immediate attention and a personalized response from management.', 
 CURRENT_TIMESTAMP - INTERVAL '3 hours', 3, 5, true, 0.92, 
 ARRAY['customer', 'complaint', 'premium', 'delivery', 'contract'], 
 'Contact ABC Corp, investigate order #12345, provide management response'),

-- Medium Priority Emails
('email-004', 'user-john', 'New Security Policy Updates', 'security@company.com', 'all-employees@company.com', 
 'Dear All, we are implementing new security policies effective next Monday. All employees must complete the security training module by Friday. Please find the training link and updated policy document attached.', 
 CURRENT_TIMESTAMP - INTERVAL '6 hours', 2, 2, false, 0.75, 
 ARRAY['security', 'policy', 'training', 'compliance', 'deadline'], 
 'Complete security training by Friday'),

('email-005', 'user-john', 'Weekly Team Standup Notes', 'team-lead@company.com', 'dev-team@company.com', 
 'Hi Team, here are the notes from today''s standup: Sprint is on track, API integration completed, UI testing in progress. Next week we will focus on performance optimization. Great work everyone!', 
 CURRENT_TIMESTAMP - INTERVAL '4 hours', 1, 10, false, 0.70, 
 ARRAY['standup', 'sprint', 'development', 'team', 'progress'], 
 null),

-- Low Priority Emails
('email-006', 'user-john', 'Monthly Newsletter - Tech Innovations', 'newsletter@techmagazine.com', 'john.doe@company.com', 
 'Discover the latest innovations in AI, cloud computing, and cybersecurity. This month''s featured articles include machine learning trends, serverless architecture benefits, and zero-trust security models.', 
 CURRENT_TIMESTAMP - INTERVAL '1 day', 0, 7, false, 0.60, 
 ARRAY['newsletter', 'technology', 'AI', 'cloud', 'cybersecurity'], 
 null),

('email-007', 'user-john', 'Lunch Invitation - New Restaurant', 'colleague@company.com', 'john.doe@company.com', 
 'Hey John! Want to try the new Mediterranean restaurant that opened downtown? I heard they have amazing falafel. Are you free this Friday around 12:30?', 
 CURRENT_TIMESTAMP - INTERVAL '2 days', 0, 9, true, 0.65, 
 ARRAY['lunch', 'invitation', 'restaurant', 'personal', 'friday'], 
 'Respond about Friday lunch availability'),

-- User 2 Emails
('email-008', 'user-jane', 'Project Deadline Extension Request', 'project-manager@company.com', 'jane.smith@company.com', 
 'Jane, the client has requested a 2-week extension for the mobile app project due to additional feature requirements. Please review the impact on our timeline and resources, and provide your recommendation by EOD tomorrow.', 
 CURRENT_TIMESTAMP - INTERVAL '5 hours', 2, 3, true, 0.85, 
 ARRAY['project', 'deadline', 'extension', 'mobile app', 'client'], 
 'Review timeline impact, provide recommendation by EOD tomorrow'),

('email-009', 'user-jane', 'Conference Speaking Opportunity', 'events@techconf.com', 'jane.smith@company.com', 
 'Dear Jane, we would like to invite you to speak at TechConf 2025 about "Modern Software Architecture Patterns". The conference is scheduled for March 15-17 in San Francisco. Speaker benefits include travel expenses and accommodation.', 
 CURRENT_TIMESTAMP - INTERVAL '1 day', 1, 1, true, 0.80, 
 ARRAY['conference', 'speaking', 'architecture', 'travel', 'opportunity'], 
 'Decide on speaking opportunity, respond to conference organizers');

-- Insert test drafts
INSERT INTO email_drafts (
    processed_email_id, user_id, generated_content, confidence_score, status, additional_context
) VALUES 
((SELECT id FROM processed_emails WHERE email_id = 'email-001'), 'user-john', 
 'Thank you for bringing this to my attention. I am immediately investigating the server performance issues. I will coordinate with the infrastructure team and provide you with a detailed status update within 30 minutes. In the meantime, I am monitoring the situation closely.', 
 0.88, 0, 'Generated response for urgent server issue'),

((SELECT id FROM processed_emails WHERE email_id = 'email-002'), 'user-john', 
 'Thank you for the reminder. I will attend the Q4 budget review meeting tomorrow at 2 PM in Conference Room A. I will bring my department''s budget proposal and Q3 expense reports as requested. Looking forward to the discussion.', 
 0.82, 0, 'Meeting confirmation response'),

((SELECT id FROM processed_emails WHERE email_id = 'email-007'), 'user-john', 
 'Hi! The Mediterranean restaurant sounds great! I am available this Friday at 12:30. Looking forward to trying their falafel. Should we meet there or would you like me to pick you up?', 
 0.75, 0, 'Casual lunch invitation response'),

((SELECT id FROM processed_emails WHERE email_id = 'email-008'), 'user-jane', 
 'I will review the client''s extension request and assess the impact on our project timeline and resource allocation. I will analyze the additional feature requirements and provide my recommendation along with an updated project plan by end of day tomorrow.', 
 0.85, 0, 'Project extension analysis response');

-- Insert user tone profiles
INSERT INTO user_tone_profiles (
    user_id, tone_characteristics, example_texts, confidence_level
) VALUES 
('user-john', 
 '{"formality": "professional", "tone": "direct", "style": "concise", "greeting": "standard", "closing": "brief", "urgency_response": "immediate"}',
 ARRAY[
   'Thank you for your email. I will investigate this immediately and provide an update within 30 minutes.',
   'I appreciate you bringing this to my attention. Let me coordinate with the team and get back to you shortly.',
   'Understood. I will review the requirements and provide my recommendation by EOD.'
 ], 
 0.85),

('user-jane', 
 '{"formality": "professional", "tone": "collaborative", "style": "detailed", "greeting": "warm", "closing": "professional", "urgency_response": "structured"}',
 ARRAY[
   'Thank you for reaching out. I would be happy to help with this initiative. Let me review the details and provide a comprehensive analysis.',
   'I appreciate the opportunity to contribute to this project. I will coordinate with my team and ensure we deliver quality results.',
   'This sounds like an excellent opportunity. I will carefully consider all aspects and provide you with a detailed response.'
 ], 
 0.82);

-- Add some additional test emails for different scenarios
INSERT INTO processed_emails (
    email_id, user_id, subject, sender, recipients, body, received_at, 
    priority, category, requires_response, confidence_score, keywords, action_items
) VALUES 
('email-010', 'user-john', 'Marketing Campaign Performance Report', 'marketing@company.com', 'leadership@company.com', 
 'Please find attached the Q4 marketing campaign performance report. Key highlights: 25% increase in conversions, 40% improvement in ROI, successful social media engagement. Detailed analysis and recommendations for Q1 are included.', 
 CURRENT_TIMESTAMP - INTERVAL '8 hours', 1, 2, false, 0.78, 
 ARRAY['marketing', 'campaign', 'performance', 'ROI', 'report'], 
 null),

('email-011', 'user-jane', 'Training Session: Advanced Docker Techniques', 'training@company.com', 'developers@company.com', 
 'Join us for an advanced Docker training session next Tuesday from 2-4 PM. Topics include multi-stage builds, optimization techniques, security best practices, and orchestration with Kubernetes. Registration link attached.', 
 CURRENT_TIMESTAMP - INTERVAL '12 hours', 1, 2, true, 0.72, 
 ARRAY['training', 'docker', 'kubernetes', 'development', 'registration'], 
 'Register for Docker training session'),

('email-012', 'user-john', 'System Maintenance Window - This Weekend', 'infrastructure@company.com', 'all-users@company.com', 
 'Scheduled maintenance this Saturday 11 PM - Sunday 3 AM EST. Services affected: email, file sharing, VPN. Emergency contact: ops-team@company.com. Please plan accordingly and save your work before the maintenance window.', 
 CURRENT_TIMESTAMP - INTERVAL '2 days', 2, 2, false, 0.88, 
 ARRAY['maintenance', 'downtime', 'weekend', 'services', 'planning'], 
 'Plan work around maintenance window');

COMMIT;

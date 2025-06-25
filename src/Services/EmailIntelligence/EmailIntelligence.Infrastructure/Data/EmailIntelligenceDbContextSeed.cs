using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EmailIntelligence.Infrastructure.Data;

public static class EmailIntelligenceDbContextSeed
{
    public static async Task SeedAsync(EmailIntelligenceDbContext context, ILogger logger)
    {
        try
        {
            if (!context.ProcessedEmails.Any())
            {
                await SeedProcessedEmailsAsync(context);
                logger.LogInformation("Seed data for ProcessedEmails has been applied");
            }

            if (!context.UserToneProfiles.Any())
            {
                await SeedUserToneProfilesAsync(context);
                logger.LogInformation("Seed data for UserToneProfiles has been applied");
            }

            if (!context.EmailDrafts.Any())
            {
                await SeedEmailDraftsAsync(context);
                logger.LogInformation("Seed data for EmailDrafts has been applied");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedProcessedEmailsAsync(EmailIntelligenceDbContext context)
    {
        var baseDate = DateTime.UtcNow.AddDays(-30);
        var userId = "user123@example.com";

        var seedEmails = new List<ProcessedEmail>
        {
            // 1. RequiresResponse
            ProcessedEmail.Create(
                emailId: "email_001",
                userId: userId,
                subject: "Project Budget Approval Needed",
                from: "project.manager@company.com",
                to: userId,
                body: "Hi, I need your approval for the Q1 project budget. The total amount is $50,000 for the new CRM implementation. Please review the attached proposal and let me know if you have any questions. We need approval by Friday to stay on schedule.",
                receivedAt: baseDate.AddDays(-2),
                priority: EmailPriority.High,
                category: EmailCategory.RequiresResponse,
                requiresResponse: true,
                confidenceScore: 0.95,
                keywords: new[] { "approval", "budget", "Friday", "deadline" },
                actionItems: "Review budget proposal and provide approval by Friday"
            ),

            // 2. Informational
            ProcessedEmail.Create(
                emailId: "email_002",
                userId: userId,
                subject: "Company Policy Update - Remote Work Guidelines",
                from: "hr@company.com",
                to: userId,
                body: "Dear Team, We're updating our remote work policy effective January 1st. Key changes include: flexible working hours between 8 AM - 6 PM, mandatory team check-ins twice weekly, and updated equipment reimbursement policies. Full details are available on the company intranet.",
                receivedAt: baseDate.AddDays(-1),
                priority: EmailPriority.Medium,
                category: EmailCategory.Informational,
                requiresResponse: false,
                confidenceScore: 0.92,
                keywords: new[] { "policy", "remote work", "guidelines", "effective January" },
                actionItems: null
            ),

            // 3. ActionRequired
            ProcessedEmail.Create(
                emailId: "email_003",
                userId: userId,
                subject: "URGENT: Security Certificate Renewal Required",
                from: "security@company.com",
                to: userId,
                body: "IMMEDIATE ACTION REQUIRED: Your security certificate expires in 3 days. Please complete the renewal process by logging into the security portal and following the certificate renewal wizard. Failure to renew will result in system access being revoked.",
                receivedAt: baseDate.AddHours(-6),
                priority: EmailPriority.Critical,
                category: EmailCategory.ActionRequired,
                requiresResponse: true,
                confidenceScore: 0.98,
                keywords: new[] { "URGENT", "security certificate", "expires", "3 days", "immediate action" },
                actionItems: "Renew security certificate within 3 days via security portal"
            ),

            // 4. Meeting
            ProcessedEmail.Create(
                emailId: "email_004",
                userId: userId,
                subject: "Weekly Team Standup - Tomorrow 10 AM",
                from: "scrum.master@company.com",
                to: userId,
                body: "Hi team, reminder that our weekly standup is scheduled for tomorrow at 10 AM in Conference Room B. We'll be discussing sprint progress, blockers, and planning for next sprint. Please come prepared with your updates. Meeting should last 30 minutes.",
                receivedAt: baseDate.AddDays(-1),
                priority: EmailPriority.Medium,
                category: EmailCategory.Meeting,
                requiresResponse: false,
                confidenceScore: 0.89,
                keywords: new[] { "standup", "tomorrow", "10 AM", "Conference Room B", "meeting" },
                actionItems: "Attend weekly standup tomorrow at 10 AM, prepare updates"
            ),

            // 5. Support
            ProcessedEmail.Create(
                emailId: "email_005",
                userId: userId,
                subject: "Ticket #12345 - Login Issue Resolved",
                from: "support@company.com",
                to: userId,
                body: "Your support ticket #12345 regarding login issues has been resolved. Our team has reset your password and updated your account permissions. You should now be able to access all systems normally. If you continue experiencing issues, please reply to this email or create a new ticket.",
                receivedAt: baseDate.AddDays(-3),
                priority: EmailPriority.Low,
                category: EmailCategory.Support,
                requiresResponse: false,
                confidenceScore: 0.91,
                keywords: new[] { "ticket", "resolved", "login issue", "password reset" },
                actionItems: null
            ),

            // 6. Marketing
            ProcessedEmail.Create(
                emailId: "email_006",
                userId: userId,
                subject: "🎉 Exclusive 30% Off - Limited Time Offer!",
                from: "promotions@retailstore.com",
                to: userId,
                body: "Don't miss out! Get 30% off all electronics this weekend only. Use code SAVE30 at checkout. Free shipping on orders over $100. Shop now for the latest smartphones, laptops, and gadgets. Offer expires Sunday at midnight.",
                receivedAt: baseDate.AddDays(-5),
                priority: EmailPriority.Low,
                category: EmailCategory.Marketing,
                requiresResponse: false,
                confidenceScore: 0.94,
                keywords: new[] { "30% off", "limited time", "SAVE30", "offer expires" },
                actionItems: null
            ),

            // 7. Newsletter
            ProcessedEmail.Create(
                emailId: "email_007",
                userId: userId,
                subject: "Tech Weekly #127 - AI Trends and Cloud Updates",
                from: "newsletter@techweekly.com",
                to: userId,
                body: "This Week in Tech: AI breakthrough in natural language processing, major cloud provider announces new services, cybersecurity threats on the rise. Featured articles: 'Machine Learning in Healthcare', 'Quantum Computing Progress', and 'Best Practices for Remote Development Teams'.",
                receivedAt: baseDate.AddDays(-7),
                priority: EmailPriority.Low,
                category: EmailCategory.Newsletter,
                requiresResponse: false,
                confidenceScore: 0.88,
                keywords: new[] { "Tech Weekly", "AI trends", "cloud updates", "featured articles" },
                actionItems: null
            ),

            // 8. Spam
            ProcessedEmail.Create(
                emailId: "email_008",
                userId: userId,
                subject: "Congratulations! You've Won $1,000,000!!!",
                from: "winner@lottery-scam.com",
                to: userId,
                body: "CONGRATULATIONS!!! You have been selected as the winner of our international lottery! You've won $1,000,000 USD! To claim your prize, please reply with your bank details and social security number. ACT NOW! This offer expires in 24 hours!",
                receivedAt: baseDate.AddDays(-10),
                priority: EmailPriority.Low,
                category: EmailCategory.Spam,
                requiresResponse: false,
                confidenceScore: 0.97,
                keywords: new[] { "won", "lottery", "bank details", "social security", "act now" },
                actionItems: null
            ),

            // 9. Personal
            ProcessedEmail.Create(
                emailId: "email_009",
                userId: userId,
                subject: "Family Reunion Planning - Save the Date",
                from: "mom@family.com",
                to: userId,
                body: "Hi honey, hope you're doing well! We're planning the annual family reunion for July 15th at Grandma's house. Please let me know if you can make it so I can plan the food accordingly. Your cousin Sarah is getting married next month, so this will be a great chance to celebrate with everyone. Love you!",
                receivedAt: baseDate.AddDays(-4),
                priority: EmailPriority.Medium,
                category: EmailCategory.Personal,
                requiresResponse: true,
                confidenceScore: 0.86,
                keywords: new[] { "family reunion", "July 15th", "Grandma's house", "let me know" },
                actionItems: "Confirm attendance for family reunion on July 15th"
            ),

            // 10. Internal
            ProcessedEmail.Create(
                emailId: "email_010",
                userId: userId,
                subject: "Internal: Server Maintenance Window - This Weekend",
                from: "devops@company.com",
                to: userId,
                body: "INTERNAL NOTICE: Scheduled server maintenance this Saturday 2-6 AM EST. Services will be temporarily unavailable during this window. Please plan accordingly and inform your teams. Database backups will be performed and security patches applied. No action required from development teams.",
                receivedAt: baseDate.AddDays(-8),
                priority: EmailPriority.Medium,
                category: EmailCategory.Internal,
                requiresResponse: false,
                confidenceScore: 0.93,
                keywords: new[] { "internal", "server maintenance", "Saturday", "2-6 AM", "unavailable" },
                actionItems: null
            ),

            // Additional emails for variety
            ProcessedEmail.Create(
                emailId: "email_011",
                userId: userId,
                subject: "Re: Contract Review - Need Your Input",
                from: "legal@company.com",
                to: userId,
                body: "Following up on the vendor contract we discussed. I've reviewed the terms and have some concerns about the liability clauses in section 7. Can we schedule a call this week to discuss? The client is expecting our response by Thursday.",
                receivedAt: baseDate.AddDays(-6),
                priority: EmailPriority.High,
                category: EmailCategory.RequiresResponse,
                requiresResponse: true,
                confidenceScore: 0.91,
                keywords: new[] { "contract review", "liability clauses", "schedule call", "Thursday" },
                actionItems: "Schedule call with legal team to discuss contract terms"
            ),

            ProcessedEmail.Create(
                emailId: "email_012",
                userId: userId,
                subject: "Quarterly All-Hands Meeting - March 15th",
                from: "ceo@company.com",
                to: userId,
                body: "Team, mark your calendars for our Q1 all-hands meeting on March 15th at 2 PM in the main auditorium. We'll be sharing company performance, new product launches, and recognizing outstanding team members. Attendance is mandatory for all full-time employees.",
                receivedAt: baseDate.AddDays(-12),
                priority: EmailPriority.High,
                category: EmailCategory.Meeting,
                requiresResponse: false,
                confidenceScore: 0.94,
                keywords: new[] { "all-hands meeting", "March 15th", "2 PM", "auditorium", "mandatory" },
                actionItems: "Attend quarterly all-hands meeting on March 15th"
            ),

            ProcessedEmail.Create(
                emailId: "email_013",
                userId: userId,
                subject: "System Outage Report - January Incidents",
                from: "monitoring@company.com",
                to: userId,
                body: "Monthly system reliability report for January: 99.8% uptime achieved. Two minor incidents: 5-minute API slowdown on Jan 12th and 15-minute database connection issue on Jan 28th. Both issues have been resolved and preventive measures implemented.",
                receivedAt: baseDate.AddDays(-15),
                priority: EmailPriority.Low,
                category: EmailCategory.Informational,
                requiresResponse: false,
                confidenceScore: 0.87,
                keywords: new[] { "system reliability", "99.8% uptime", "minor incidents", "resolved" },
                actionItems: null
            )
        };

        await context.ProcessedEmails.AddRangeAsync(seedEmails);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUserToneProfilesAsync(EmailIntelligenceDbContext context)
    {
        var userToneProfiles = new List<UserToneProfile>
        {
            UserToneProfile.Create(
                userId: "user123@example.com",
                toneCharacteristics: "Professional, concise, friendly. Prefers bullet points for action items. Uses collaborative language.",
                writingStyle: "Professional"
            ),
            UserToneProfile.Create(
                userId: "manager@company.com",
                toneCharacteristics: "Direct, authoritative, results-oriented. Focuses on deadlines and deliverables. Formal communication style.",
                writingStyle: "Formal"
            ),
            UserToneProfile.Create(
                userId: "developer@company.com",
                toneCharacteristics: "Technical, detailed, collaborative. Includes code examples and technical specifications. Casual but informative.",
                writingStyle: "Casual"
            )
        };

        // Update profiles to set realistic confidence levels and email samples
        userToneProfiles[0].UpdateProfile(
            userToneProfiles[0].ToneCharacteristics,
            "[\"Thanks for your time\", \"Let me know if you have questions\", \"Happy to discuss further\"]",
            "[\"ASAP\", \"Urgent\", \"Immediately\"]",
            "Professional",
            15 // emails sampled
        );

        userToneProfiles[1].UpdateProfile(
            userToneProfiles[1].ToneCharacteristics,
            "[\"Please provide\", \"Required by\", \"Deliverable\", \"Action required\"]",
            "[\"Maybe\", \"I think\", \"Possibly\"]",
            "Formal",
            25 // emails sampled
        );

        userToneProfiles[2].UpdateProfile(
            userToneProfiles[2].ToneCharacteristics,
            "[\"Here's how\", \"Quick update\", \"FYI\", \"Let's sync\"]",
            "[\"Per company policy\", \"As per\", \"Aforementioned\"]",
            "Casual",
            12 // emails sampled
        );

        await context.UserToneProfiles.AddRangeAsync(userToneProfiles);
        await context.SaveChangesAsync();
    }

    private static async Task SeedEmailDraftsAsync(EmailIntelligenceDbContext context)
    {
        // Get some processed emails to create drafts for
        var processedEmails = await context.ProcessedEmails
            .Where(e => e.RequiresResponse)
            .Take(5)
            .ToListAsync();

        if (!processedEmails.Any())
            return;

        var userId = "user123@example.com";
        var emailDrafts = new List<EmailDraft>();

        // Draft for budget approval email
        var budgetEmail = processedEmails.FirstOrDefault(e => e.Subject.Contains("Budget"));
        if (budgetEmail != null)
        {
            var budgetDraft = EmailDraft.Create(
                processedEmailId: budgetEmail.Id,
                userId: userId,
                originalContent: budgetEmail.Body,
                generatedContent: @"Hi [Project Manager],

Thank you for sending the Q1 project budget proposal. I've reviewed the $50,000 budget for the new CRM implementation.

The proposal looks comprehensive and well-structured. I have a few questions:
• Can you provide a breakdown of the implementation phases?
• What contingency measures are in place if we encounter technical challenges?
• Is training for the team included in this budget?

Assuming these details can be clarified, I'm prepared to approve the budget by Thursday to keep us on schedule.

Best regards,
[Your name]",
                modelUsed: "gpt-4",
                prompt: "Generate a professional response to budget approval request, asking for clarification while showing willingness to approve",
                generationConfidence: 0.91,
                qualityScore: 0.87
            );

            // Simulate user editing the draft
            budgetDraft.EditByUser(@"Hi [Project Manager],

Thank you for the Q1 project budget proposal. I've reviewed the $50,000 budget for the CRM implementation.

The proposal looks solid. Before I approve, could you please clarify:
• Implementation timeline and phases
• Risk mitigation strategies
• Training costs inclusion

I'm happy to approve by Thursday to maintain our schedule.

Thanks,
[Your name]", new[] { "tone", "structure", "brevity" });

            budgetDraft.Approve();
            emailDrafts.Add(budgetDraft);
        }

        // Draft for contract review email
        var contractEmail = processedEmails.FirstOrDefault(e => e.Subject.Contains("Contract"));
        if (contractEmail != null)
        {
            var contractDraft = EmailDraft.Create(
                processedEmailId: contractEmail.Id,
                userId: userId,
                originalContent: contractEmail.Body,
                generatedContent: @"Hi [Legal Team],

Thanks for reviewing the vendor contract and flagging the liability concerns in section 7.

I share your concerns about those clauses. Let's schedule a call to discuss:
• Specific liability limitations we need
• Alternative language proposals
• Impact on project timeline

I'm available Tuesday-Thursday this week, morning preferred. The client deadline is Thursday, so we should aim for Tuesday or Wednesday.

Please send a calendar invite when convenient.

Best,
[Your name]",
                modelUsed: "gpt-4",
                prompt: "Generate response to legal team about contract review, showing urgency while being collaborative",
                generationConfidence: 0.89,
                qualityScore: 0.85
            );

            emailDrafts.Add(contractDraft);
        }

        // Draft for family reunion email
        var familyEmail = processedEmails.FirstOrDefault(e => e.Subject.Contains("Family"));
        if (familyEmail != null)
        {
            var familyDraft = EmailDraft.Create(
                processedEmailId: familyEmail.Id,
                userId: userId,
                originalContent: familyEmail.Body,
                generatedContent: @"Hi Mom,

Hope you're doing well too! 

I'm excited about the family reunion on July 15th at Grandma's house. Yes, I can definitely make it! Please count me in for the planning.

That's wonderful news about Sarah's engagement! It'll be perfect timing to celebrate with everyone together.

Let me know if you need help with anything for the reunion - I'm happy to contribute to food, decorations, or whatever else you might need.

Love you too!
[Your name]",
                modelUsed: "gpt-4",
                prompt: "Generate warm, personal response to family reunion invitation, showing enthusiasm and offering help",
                generationConfidence: 0.82,
                qualityScore: 0.79
            );

            // Simulate user editing for more personal touch
            familyDraft.EditByUser(@"Hi Mom! ❤️

Great to hear from you! 

Absolutely yes to July 15th - wouldn't miss it! Count me in and I'm already looking forward to seeing everyone.

So exciting about Sarah getting married! This reunion timing is perfect for celebrating together.

I'd love to help with the reunion - maybe I can bring my famous potato salad or help with setup? Just let me know what works best.

Can't wait to see you all!

Love,
[Your name]", new[] { "tone", "personalization", "enthusiasm" });

            familyDraft.Approve();
            emailDrafts.Add(familyDraft);
        }

        // Draft for security certificate renewal
        var securityEmail = processedEmails.FirstOrDefault(e => e.Subject.Contains("Security") || e.Subject.Contains("URGENT"));
        if (securityEmail != null)
        {
            var securityDraft = EmailDraft.Create(
                processedEmailId: securityEmail.Id,
                userId: userId,
                originalContent: securityEmail.Body,
                generatedContent: @"Hi Security Team,

Thank you for the urgent reminder about my security certificate expiration.

I understand the certificate expires in 3 days and will complete the renewal process immediately. I will:

1. Log into the security portal today
2. Complete the certificate renewal wizard
3. Confirm successful renewal with your team

I'll follow up once the renewal is complete to ensure everything is properly updated in your systems.

Thanks for the proactive notification.

Best regards,
[Your name]",
                modelUsed: "gpt-4",
                prompt: "Generate prompt acknowledgment of urgent security requirement with clear action plan",
                generationConfidence: 0.94,
                qualityScore: 0.91
            );

            securityDraft.MarkAsSent();
            emailDrafts.Add(securityDraft);
        }

        await context.EmailDrafts.AddRangeAsync(emailDrafts);
        await context.SaveChangesAsync();
    }
}

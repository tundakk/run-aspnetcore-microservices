using Microsoft.EntityFrameworkCore;
using EmailIntelligence.Domain.Entities;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace EmailIntelligence.Infrastructure.Data;

public class EmailIntelligenceDbContext : DbContext
{
    public EmailIntelligenceDbContext(DbContextOptions<EmailIntelligenceDbContext> options) : base(options)
    {
    }

    public DbSet<ProcessedEmail> ProcessedEmails { get; set; } = null!;
    public DbSet<EmailDraft> EmailDrafts { get; set; } = null!;
    public DbSet<UserToneProfile> UserToneProfiles { get; set; } = null!;
    public DbSet<EmailEmbedding> EmailEmbeddings { get; set; } = null!;
    public DbSet<LearningPattern> LearningPatterns { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);        modelBuilder.Entity<ProcessedEmail>(entity =>
        {
            entity.ToTable("processed_emails");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmailId).HasColumnName("email_id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.Subject).HasColumnName("subject").IsRequired();
            entity.Property(e => e.From).HasColumnName("sender").IsRequired();
            entity.Property(e => e.To).HasColumnName("recipients").IsRequired();
            entity.Property(e => e.Body).HasColumnName("body").IsRequired();
            entity.Property(e => e.ReceivedAt).HasColumnName("received_at").IsRequired();
            entity.Property(e => e.Priority).HasColumnName("priority").IsRequired();
            entity.Property(e => e.Category).HasColumnName("category").IsRequired();
            entity.Property(e => e.RequiresResponse).HasColumnName("requires_response").IsRequired();
            entity.Property(e => e.ConfidenceScore).HasColumnName("confidence_score").IsRequired();
            entity.Property(e => e.ExtractedKeywords).HasColumnName("keywords");
            entity.Property(e => e.ActionItems).HasColumnName("action_items");
            entity.Property(e => e.ProcessedAt).HasColumnName("processed_at").IsRequired();
            
            // Ignore properties that don't exist in database
            entity.Ignore(e => e.UserCorrectedPriority);
            entity.Ignore(e => e.UserCorrectedPriorityValue);
            entity.Ignore(e => e.UserCorrectedCategory);
            entity.Ignore(e => e.UserCorrectedCategoryValue);
        });        modelBuilder.Entity<EmailDraft>(entity =>
        {
            entity.ToTable("email_drafts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ProcessedEmailId).HasColumnName("processed_email_id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.GeneratedContent).HasColumnName("generated_content").IsRequired();
            entity.Property(e => e.UserEditedContent).HasColumnName("edited_content");
            entity.Property(e => e.Status).HasColumnName("status").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.GenerationConfidence).HasColumnName("confidence_score").IsRequired();
            entity.Property(e => e.UserEditTypes).HasColumnName("edit_types");

            // Ignore properties that don't exist in database
            entity.Ignore(e => e.OriginalContent);
            entity.Ignore(e => e.FinalContent);
            entity.Ignore(e => e.EditedAt);
            entity.Ignore(e => e.SentAt);
            entity.Ignore(e => e.OriginalQualityScore);
            entity.Ignore(e => e.UserEditCount);
            entity.Ignore(e => e.WasApproved);
            entity.Ignore(e => e.ModelUsed);
            entity.Ignore(e => e.Prompt);

            entity.HasOne(d => d.ProcessedEmail)
                .WithMany()
                .HasForeignKey(d => d.ProcessedEmailId)
                .OnDelete(DeleteBehavior.Cascade);
        });        modelBuilder.Entity<UserToneProfile>(entity =>
        {
            entity.ToTable("user_tone_profiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.ToneCharacteristics).HasColumnName("tone_characteristics").IsRequired();
            entity.Property(e => e.ConfidenceLevel).HasColumnName("confidence_level").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.LastUpdated).HasColumnName("updated_at").IsRequired();

            // Ignore properties that don't exist in database
            entity.Ignore(e => e.PreferredPhrases);
            entity.Ignore(e => e.AvoidedPhrases);
            entity.Ignore(e => e.WritingStyle);
            entity.Ignore(e => e.EmailsSampled);
        });

        // EmailEmbedding entity configuration
        modelBuilder.Entity<EmailEmbedding>(entity =>
        {
            entity.ToTable("email_embeddings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmailId).HasColumnName("email_id").IsRequired();
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.ContentType).HasColumnName("content_type").IsRequired();
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.Embedding).HasColumnName("embedding").IsRequired();
            entity.Property(e => e.Metadata)
                .HasColumnName("metadata")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").IsRequired();
            
            entity.HasIndex(e => e.UserId).HasDatabaseName("ix_email_embeddings_user_id");
            entity.HasIndex(e => e.EmailId).HasDatabaseName("ix_email_embeddings_email_id");
            entity.HasIndex(e => e.ContentType).HasDatabaseName("ix_email_embeddings_content_type");
            // Note: Vector index will be created in migration
        });

        // LearningPattern entity configuration
        modelBuilder.Entity<LearningPattern>(entity =>
        {
            entity.ToTable("learning_patterns");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.UserId).HasColumnName("user_id").IsRequired();
            entity.Property(e => e.PatternType).HasColumnName("pattern_type").IsRequired();
            entity.Property(e => e.OriginalContent).HasColumnName("original_content").IsRequired();
            entity.Property(e => e.ModifiedContent).HasColumnName("modified_content").IsRequired();
            entity.Property(e => e.SemanticDifference).HasColumnName("semantic_difference").IsRequired();
            entity.Property(e => e.Context)
                .HasColumnName("context")
                .HasColumnType("jsonb")
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>());
            entity.Property(e => e.ConfidenceScore).HasColumnName("confidence_score").IsRequired();
            entity.Property(e => e.UsageCount).HasColumnName("usage_count").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
            entity.Property(e => e.LastUsedAt).HasColumnName("last_used_at").IsRequired();
            
            entity.HasIndex(e => e.UserId).HasDatabaseName("ix_learning_patterns_user_id");
            entity.HasIndex(e => e.PatternType).HasDatabaseName("ix_learning_patterns_pattern_type");
            entity.HasIndex(e => e.ConfidenceScore).HasDatabaseName("ix_learning_patterns_confidence_score");
        });
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmailIntelligence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "email_embeddings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    content_type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    embedding = table.Column<float[]>(type: "real[]", nullable: false),
                    metadata = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_embeddings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "learning_patterns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    pattern_type = table.Column<string>(type: "text", nullable: false),
                    original_content = table.Column<string>(type: "text", nullable: false),
                    modified_content = table.Column<string>(type: "text", nullable: false),
                    semantic_difference = table.Column<float[]>(type: "real[]", nullable: false),
                    context = table.Column<string>(type: "jsonb", nullable: false),
                    confidence_score = table.Column<double>(type: "double precision", nullable: false),
                    usage_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_used_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_learning_patterns", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processed_emails",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email_id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    subject = table.Column<string>(type: "text", nullable: false),
                    sender = table.Column<string>(type: "text", nullable: false),
                    recipients = table.Column<string>(type: "text", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    received_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    requires_response = table.Column<bool>(type: "boolean", nullable: false),
                    confidence_score = table.Column<double>(type: "double precision", nullable: false),
                    keywords = table.Column<string[]>(type: "text[]", nullable: false),
                    action_items = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processed_emails", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_tone_profiles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    tone_characteristics = table.Column<string>(type: "text", nullable: false),
                    confidence_level = table.Column<double>(type: "double precision", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_tone_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "email_drafts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    processed_email_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    generated_content = table.Column<string>(type: "text", nullable: false),
                    edited_content = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    edit_types = table.Column<string[]>(type: "text[]", nullable: true),
                    confidence_score = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_email_drafts", x => x.id);
                    table.ForeignKey(
                        name: "FK_email_drafts_processed_emails_processed_email_id",
                        column: x => x.processed_email_id,
                        principalTable: "processed_emails",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_email_drafts_processed_email_id",
                table: "email_drafts",
                column: "processed_email_id");

            migrationBuilder.CreateIndex(
                name: "ix_email_embeddings_content_type",
                table: "email_embeddings",
                column: "content_type");

            migrationBuilder.CreateIndex(
                name: "ix_email_embeddings_email_id",
                table: "email_embeddings",
                column: "email_id");

            migrationBuilder.CreateIndex(
                name: "ix_email_embeddings_user_id",
                table: "email_embeddings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_learning_patterns_confidence_score",
                table: "learning_patterns",
                column: "confidence_score");

            migrationBuilder.CreateIndex(
                name: "ix_learning_patterns_pattern_type",
                table: "learning_patterns",
                column: "pattern_type");

            migrationBuilder.CreateIndex(
                name: "ix_learning_patterns_user_id",
                table: "learning_patterns",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_drafts");

            migrationBuilder.DropTable(
                name: "email_embeddings");

            migrationBuilder.DropTable(
                name: "learning_patterns");

            migrationBuilder.DropTable(
                name: "user_tone_profiles");

            migrationBuilder.DropTable(
                name: "processed_emails");
        }
    }
}

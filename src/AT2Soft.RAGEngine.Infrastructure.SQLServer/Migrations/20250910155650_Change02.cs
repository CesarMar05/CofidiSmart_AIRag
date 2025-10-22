using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class Change02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_KnowledgeDocuments_ApplicationClientId",
                table: "KnowledgeDocuments");

            migrationBuilder.CreateTable(
                name: "RagIngestJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TextContent = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    TextLength = table.Column<int>(type: "int", nullable: false),
                    TextChunkerOptions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metadata = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Error = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Digest = table.Column<string>(type: "nvarchar(24)", maxLength: 24, nullable: false),
                    ChunksProcess = table.Column<int>(type: "int", nullable: false),
                    ChunksTotal = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RagIngestJobs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_ApplicationClientId_Digest",
                table: "KnowledgeDocuments",
                columns: new[] { "ApplicationClientId", "Digest" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RagIngestJobs_ApplicationId_Digest",
                table: "RagIngestJobs",
                columns: new[] { "ApplicationId", "Digest" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RagIngestJobs");

            migrationBuilder.DropIndex(
                name: "IX_KnowledgeDocuments_ApplicationClientId_Digest",
                table: "KnowledgeDocuments");

            migrationBuilder.CreateIndex(
                name: "IX_KnowledgeDocuments_ApplicationClientId",
                table: "KnowledgeDocuments",
                column: "ApplicationClientId");
        }
    }
}

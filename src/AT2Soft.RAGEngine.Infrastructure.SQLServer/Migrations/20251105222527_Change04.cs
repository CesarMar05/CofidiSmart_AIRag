using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class Change04 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationClientRAGConfigs",
                columns: table => new
                {
                    ApplicationClientPromptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Prompt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TargetTokens = table.Column<int>(type: "int", nullable: false),
                    MaxTokens = table.Column<int>(type: "int", nullable: false),
                    MinTokens = table.Column<int>(type: "int", nullable: false),
                    OverlapTokens = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationClientRAGConfigs", x => x.ApplicationClientPromptId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationClientRAGConfigs_ApplicationClientId_Tenant",
                table: "ApplicationClientRAGConfigs",
                columns: new[] { "ApplicationClientId", "Tenant" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationClientRAGConfigs");
        }
    }
}

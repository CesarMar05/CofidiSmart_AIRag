using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AT2Soft.RAGEngine.Infrastructure.SQLServer.Migrations
{
    /// <inheritdoc />
    public partial class Change01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedTokens",
                table: "Chunks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedTokens",
                table: "Chunks");
        }
    }
}

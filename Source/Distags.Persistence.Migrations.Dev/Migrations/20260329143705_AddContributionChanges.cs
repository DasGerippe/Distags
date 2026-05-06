using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataStoringMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "changes",
                table: "contributions",
                type: "jsonb",
                nullable: false,
                defaultValue: "{}");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "changes",
                table: "contributions");
        }
    }
}

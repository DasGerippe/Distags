using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataStoringMigrations.Migrations
{
    /// <inheritdoc />
    public partial class RenameAcceptedContributionGuidelines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "accepted_contribution_guidelines",
                table: "users",
                newName: "accepts_guidelines");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "accepts_guidelines",
                table: "users",
                newName: "accepted_contribution_guidelines");
        }
    }
}

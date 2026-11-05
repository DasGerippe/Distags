using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContributionRevertedProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "reverted",
                table: "contributions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reverted",
                table: "contributions");
        }
    }
}

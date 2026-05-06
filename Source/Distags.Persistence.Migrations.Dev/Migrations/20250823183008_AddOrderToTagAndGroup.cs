using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataStoringMigrations.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderToTagAndGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "tags",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "order",
                table: "tag_groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "order",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "order",
                table: "tag_groups");
        }
    }
}

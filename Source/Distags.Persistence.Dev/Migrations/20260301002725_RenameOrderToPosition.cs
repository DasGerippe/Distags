using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderToPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "order",
                table: "tags",
                newName: "position");

            migrationBuilder.RenameColumn(
                name: "order",
                table: "tag_groups",
                newName: "position");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "position",
                table: "tags",
                newName: "order");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "tag_groups",
                newName: "order");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelWorkshopTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "workshop_tags",
                table: "levels",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "workshop_tags",
                table: "levels");
        }
    }
}

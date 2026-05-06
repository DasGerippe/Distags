using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataStoringMigrations.Migrations
{
    /// <inheritdoc />
    public partial class FlattenTagGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tag_groups_tag_groups_parent_group_id",
                table: "tag_groups");

            migrationBuilder.DropIndex(
                name: "ix_tag_groups_parent_group_id",
                table: "tag_groups");

            migrationBuilder.DropColumn(
                name: "compatibility",
                table: "tag_groups");

            migrationBuilder.DropColumn(
                name: "parent_group_id",
                table: "tag_groups");

            migrationBuilder.AddColumn<int>(
                name: "exclusivity",
                table: "tags",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "exclusivity",
                table: "tags");

            migrationBuilder.AddColumn<int>(
                name: "compatibility",
                table: "tag_groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "parent_group_id",
                table: "tag_groups",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_tag_groups_parent_group_id",
                table: "tag_groups",
                column: "parent_group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tag_groups_tag_groups_parent_group_id",
                table: "tag_groups",
                column: "parent_group_id",
                principalTable: "tag_groups",
                principalColumn: "id");
        }
    }
}

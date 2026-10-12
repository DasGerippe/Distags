using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class TagDisplayNameAndParentGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "tags",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tag_groups_tag_groups_parent_group_id",
                table: "tag_groups");

            migrationBuilder.DropIndex(
                name: "ix_tag_groups_parent_group_id",
                table: "tag_groups");

            migrationBuilder.DropColumn(
                name: "display_name",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "parent_group_id",
                table: "tag_groups");
        }
    }
}

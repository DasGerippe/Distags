using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagToContributionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "tag_id",
                table: "contributions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_contributions_tag_id",
                table: "contributions",
                column: "tag_id");

            migrationBuilder.AddForeignKey(
                name: "fk_contributions_tags_tag_id",
                table: "contributions",
                column: "tag_id",
                principalTable: "tags",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contributions_tags_tag_id",
                table: "contributions");

            migrationBuilder.DropIndex(
                name: "ix_contributions_tag_id",
                table: "contributions");

            migrationBuilder.DropColumn(
                name: "tag_id",
                table: "contributions");
        }
    }
}

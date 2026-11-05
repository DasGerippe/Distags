using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTagGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "group_id",
                table: "tags",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tag_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    compatibility = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag_groups", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tags_group_id",
                table: "tags",
                column: "group_id");

            migrationBuilder.AddForeignKey(
                name: "fk_tags_tag_groups_group_id",
                table: "tags",
                column: "group_id",
                principalTable: "tag_groups",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_tag_groups_group_id",
                table: "tags");

            migrationBuilder.DropTable(
                name: "tag_groups");

            migrationBuilder.DropIndex(
                name: "ix_tags_group_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "group_id",
                table: "tags");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class SplitTagGroup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_tag_groups_group_id",
                table: "tags");

            migrationBuilder.DropTable(
                name: "tag_groups");

            migrationBuilder.DropColumn(
                name: "exclusivity",
                table: "tags");

            migrationBuilder.RenameColumn(
                name: "group_id",
                table: "tags",
                newName: "logic_group_id");

            migrationBuilder.RenameIndex(
                name: "ix_tags_group_id",
                table: "tags",
                newName: "ix_tags_logic_group_id");

            migrationBuilder.AddColumn<long>(
                name: "display_group_id",
                table: "tags",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "is_exclusive",
                table: "tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_fallback",
                table: "tags",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "display_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_display_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "logic_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    all_exclusive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logic_groups", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tags_display_group_id",
                table: "tags",
                column: "display_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_display_groups_name",
                table: "display_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_logic_groups_name",
                table: "logic_groups",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_tags_display_groups_display_group_id",
                table: "tags",
                column: "display_group_id",
                principalTable: "display_groups",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_tags_logic_groups_logic_group_id",
                table: "tags",
                column: "logic_group_id",
                principalTable: "logic_groups",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_tags_display_groups_display_group_id",
                table: "tags");

            migrationBuilder.DropForeignKey(
                name: "fk_tags_logic_groups_logic_group_id",
                table: "tags");

            migrationBuilder.DropTable(
                name: "display_groups");

            migrationBuilder.DropTable(
                name: "logic_groups");

            migrationBuilder.DropIndex(
                name: "ix_tags_display_group_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "display_group_id",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "is_exclusive",
                table: "tags");

            migrationBuilder.DropColumn(
                name: "is_fallback",
                table: "tags");

            migrationBuilder.RenameColumn(
                name: "logic_group_id",
                table: "tags",
                newName: "group_id");

            migrationBuilder.RenameIndex(
                name: "ix_tags_logic_group_id",
                table: "tags",
                newName: "ix_tags_group_id");

            migrationBuilder.AddColumn<int>(
                name: "exclusivity",
                table: "tags",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "tag_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    description = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tag_groups", x => x.id);
                });

            migrationBuilder.AddForeignKey(
                name: "fk_tags_tag_groups_group_id",
                table: "tags",
                column: "group_id",
                principalTable: "tag_groups",
                principalColumn: "id");
        }
    }
}

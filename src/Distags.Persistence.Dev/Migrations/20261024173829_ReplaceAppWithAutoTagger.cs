using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceAppWithAutoTagger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contributions_apps_app_id",
                table: "contributions");

            migrationBuilder.DropTable(
                name: "apps");

            migrationBuilder.RenameColumn(
                name: "app_id",
                table: "contributions",
                newName: "auto_tagger_id");

            migrationBuilder.RenameIndex(
                name: "ix_contributions_app_id",
                table: "contributions",
                newName: "ix_contributions_auto_tagger_id");

            migrationBuilder.CreateTable(
                name: "auto_taggers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auto_taggers", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auto_taggers_guid",
                table: "auto_taggers",
                column: "guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_contributions_auto_taggers_auto_tagger_id",
                table: "contributions",
                column: "auto_tagger_id",
                principalTable: "auto_taggers",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contributions_auto_taggers_auto_tagger_id",
                table: "contributions");

            migrationBuilder.DropTable(
                name: "auto_taggers");

            migrationBuilder.RenameColumn(
                name: "auto_tagger_id",
                table: "contributions",
                newName: "app_id");

            migrationBuilder.RenameIndex(
                name: "ix_contributions_auto_tagger_id",
                table: "contributions",
                newName: "ix_contributions_app_id");

            migrationBuilder.CreateTable(
                name: "apps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    api_key = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apps", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_apps_api_key",
                table: "apps",
                column: "api_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_apps_name",
                table: "apps",
                column: "name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_contributions_apps_app_id",
                table: "contributions",
                column: "app_id",
                principalTable: "apps",
                principalColumn: "id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAppToContribution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "app_id",
                table: "contributions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_contributions_app_id",
                table: "contributions",
                column: "app_id");

            migrationBuilder.AddForeignKey(
                name: "fk_contributions_apps_app_id",
                table: "contributions",
                column: "app_id",
                principalTable: "apps",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_contributions_apps_app_id",
                table: "contributions");

            migrationBuilder.DropIndex(
                name: "ix_contributions_app_id",
                table: "contributions");

            migrationBuilder.DropColumn(
                name: "app_id",
                table: "contributions");
        }
    }
}

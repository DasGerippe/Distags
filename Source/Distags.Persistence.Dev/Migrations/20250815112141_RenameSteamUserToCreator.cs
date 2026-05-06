using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class RenameSteamUserToCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_steam_users",
                table: "steam_users");

            migrationBuilder.RenameTable(
                name: "steam_users",
                newName: "creators");

            migrationBuilder.AddPrimaryKey(
                name: "pk_creators",
                table: "creators",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_creators",
                table: "creators");

            migrationBuilder.RenameTable(
                name: "creators",
                newName: "steam_users");

            migrationBuilder.AddPrimaryKey(
                name: "pk_steam_users",
                table: "steam_users",
                column: "id");
        }
    }
}

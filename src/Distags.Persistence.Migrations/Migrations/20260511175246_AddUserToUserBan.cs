using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserToUserBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "user_bans",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_bans_user_id",
                table: "user_bans",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_bans_users_user_id",
                table: "user_bans",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_bans_users_user_id",
                table: "user_bans");

            migrationBuilder.DropIndex(
                name: "ix_user_bans_user_id",
                table: "user_bans");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "user_bans");
        }
    }
}

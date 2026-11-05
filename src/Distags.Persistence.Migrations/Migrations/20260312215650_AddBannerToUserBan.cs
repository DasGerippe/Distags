using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBannerToUserBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "banner_id",
                table: "user_bans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_user_bans_banner_id",
                table: "user_bans",
                column: "banner_id");

            migrationBuilder.AddForeignKey(
                name: "fk_user_bans_users_banner_id",
                table: "user_bans",
                column: "banner_id",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_bans_users_banner_id",
                table: "user_bans");

            migrationBuilder.DropIndex(
                name: "ix_user_bans_banner_id",
                table: "user_bans");

            migrationBuilder.DropColumn(
                name: "banner_id",
                table: "user_bans");
        }
    }
}

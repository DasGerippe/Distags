using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyUserSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_sessions",
                table: "user_sessions");

            migrationBuilder.DropIndex(
                name: "ix_user_sessions_refresh_token",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "id",
                table: "user_sessions");

            migrationBuilder.DropColumn(
                name: "expires_on",
                table: "user_sessions");

            migrationBuilder.RenameColumn(
                name: "refresh_token",
                table: "user_sessions",
                newName: "token");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_sessions",
                table: "user_sessions",
                column: "token");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_sessions",
                table: "user_sessions");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "user_sessions",
                newName: "refresh_token");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "user_sessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "expires_on",
                table: "user_sessions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_sessions",
                table: "user_sessions",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_refresh_token",
                table: "user_sessions",
                column: "refresh_token",
                unique: true);
        }
    }
}

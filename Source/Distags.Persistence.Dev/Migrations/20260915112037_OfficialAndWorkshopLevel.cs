using Distags.Contracts;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class OfficialAndWorkshopLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // add type column
            migrationBuilder.AddColumn<int>(
                name: "type",
                table: "levels",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // add official level columns
            migrationBuilder.AddColumn<string>(
                name: "slug",
                table: "levels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "creator_name",
                table: "levels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "game_modes",
                table: "levels",
                type: "text[]",
                nullable: true);

            // add workshop level columns and make existing columns nullable
            migrationBuilder.AddColumn<long>(
                name: "workshop_id",
                table: "levels",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "workshop_tags",
                table: "levels",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "text[]");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_on",
                table: "levels",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "levels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "file_url",
                table: "levels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                table: "levels",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<long>(
                name: "creator_id",
                table: "levels",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_on",
                table: "levels",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            // update existing (workshop) levels
            migrationBuilder.Sql("UPDATE levels SET workshop_id = id;");
            migrationBuilder.Sql($"UPDATE levels SET type = {(int)LevelType.Workshop}");

            // create unique indexes for level type specific identification columns
            migrationBuilder.CreateIndex(
                name: "ix_levels_slug",
                table: "levels",
                column: "slug",
                unique: true,
                filter: "slug IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_levels_workshop_id",
                table: "levels",
                column: "workshop_id",
                unique: true,
                filter: "workshop_id IS NOT NULL");

            // make id an identity column
            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "levels",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            // make sure the new identity sequence starts past the existing ids.
            migrationBuilder.Sql("SELECT setval(pg_get_serial_sequence('levels', 'id'), COALESCE((SELECT MAX(id) FROM levels), 1));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_levels_slug",
                table: "levels");

            migrationBuilder.DropIndex(
                name: "ix_levels_workshop_id",
                table: "levels");

            migrationBuilder.DropColumn(
                name: "creator_name",
                table: "levels");

            migrationBuilder.DropColumn(
                name: "game_modes",
                table: "levels");

            migrationBuilder.DropColumn(
                name: "slug",
                table: "levels");

            migrationBuilder.DropColumn(
                name: "type",
                table: "levels");

            migrationBuilder.DropColumn(
                name: "workshop_id",
                table: "levels");

            migrationBuilder.AlterColumn<List<string>>(
                name: "workshop_tags",
                table: "levels",
                type: "text[]",
                nullable: false,
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "updated_on",
                table: "levels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "image_url",
                table: "levels",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "file_url",
                table: "levels",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "file_name",
                table: "levels",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "creator_id",
                table: "levels",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_on",
                table: "levels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)),
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "levels",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoTaggerLastRunOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_run_on",
                table: "auto_taggers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_run_on",
                table: "auto_taggers");
        }
    }
}

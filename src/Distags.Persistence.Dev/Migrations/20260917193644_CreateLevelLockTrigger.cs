using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Dev.Migrations
{
    /// <inheritdoc />
    public partial class CreateLevelLockTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlEmbeddedFile("Database.Triggers.CreateTriggerCreateLevelLock.sql");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlEmbeddedFile("Database.Triggers.DropTriggerCreateLevelLock.sql");
        }
    }
}

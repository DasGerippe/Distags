using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddLevelLockTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlEmbeddedFile("Database.Triggers.CreateTriggerCreateLevelLock.sql");

            migrationBuilder.Sql(
                """
                insert into level_locks (level_id)
                select id
                from levels
                on conflict (level_id) do nothing;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SqlEmbeddedFile("Database.Triggers.DropTriggerCreateLevelLock.sql");
        }
    }
}

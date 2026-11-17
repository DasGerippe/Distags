using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RenameAcceleracers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update tags set name = 'hot-wheels' where name = 'acceleracers';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("update tags set name = 'acceleracers' where name = 'hot-wheels';");
        }
    }
}

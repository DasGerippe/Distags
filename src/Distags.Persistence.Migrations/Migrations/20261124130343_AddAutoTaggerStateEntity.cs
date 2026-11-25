using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AddAutoTaggerStateEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auto_tagger_states",
                columns: table => new
                {
                    auto_tagger_id = table.Column<long>(type: "bigint", nullable: false),
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auto_tagger_states", x => new { x.auto_tagger_id, x.key });
                    table.ForeignKey(
                        name: "fk_auto_tagger_states_auto_taggers_auto_tagger_id",
                        column: x => x.auto_tagger_id,
                        principalTable: "auto_taggers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.Sql(
                """
                insert into auto_tagger_states (auto_tagger_id, key, value)
                select id, '', state
                from auto_taggers
                where state is not null;
                """);

            migrationBuilder.DropColumn(
                name: "state",
                table: "auto_taggers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "state",
                table: "auto_taggers",
                type: "jsonb",
                nullable: true);

            migrationBuilder.Sql(
                """
                update auto_taggers as t
                set t.state = s.value
                from (
                    select distinct on (auto_tagger_id) auto_tagger_id, value
                    from auto_tagger_states
                    order by auto_tagger_id, key asc
                ) as s
                where s.auto_tagger_id = t.id;
                """);

            migrationBuilder.DropTable(
                name: "auto_tagger_states");
        }
    }
}

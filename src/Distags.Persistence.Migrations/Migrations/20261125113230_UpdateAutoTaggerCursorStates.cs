using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Distags.Persistence.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAutoTaggerCursorStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                update  auto_tagger_states
                set     value = jsonb_build_object('Cursor', value)
                from    auto_taggers
                where   value is not null
                    and key = ''
                    and auto_tagger_id = id
                    and guid in (
                        '2E6677C5-DF38-498A-843E-0468B21CFF94',
                        'AD96F858-91F3-46E7-B2C2-19DAFBA0DFDF',
                        '2F5E210F-22C8-4BD3-B2C3-35655E3EC8F0',
                        '074598C5-1C0A-4302-95C4-F93B5572E399');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                update  auto_tagger_states
                set     value = value -> 'Cursor'
                from    auto_taggers
                where   value ? 'Cursor'
                    and key = ''
                    and auto_tagger_id = id
                    and guid in (
                        '2E6677C5-DF38-498A-843E-0468B21CFF94',
                        'AD96F858-91F3-46E7-B2C2-19DAFBA0DFDF',
                        '2F5E210F-22C8-4BD3-B2C3-35655E3EC8F0',
                        '074598C5-1C0A-4302-95C4-F93B5572E399');
                """);
        }
    }
}

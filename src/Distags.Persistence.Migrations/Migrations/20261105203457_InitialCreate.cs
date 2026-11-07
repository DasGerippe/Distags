using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Distags.Persistence.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "auto_taggers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    guid = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    last_run_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    state = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_auto_taggers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "creators",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    avatar_hash = table.Column<string>(type: "text", nullable: false),
                    cached_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_creators", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "display_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_display_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "level_locks",
                columns: table => new
                {
                    level_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_level_locks", x => x.level_id);
                });

            migrationBuilder.CreateTable(
                name: "logic_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    all_exclusive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_logic_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    key = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_settings", x => x.key);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    steam_id = table.Column<long>(type: "bigint", nullable: true),
                    steam_name = table.Column<string>(type: "text", nullable: true),
                    steam_avatar_hash = table.Column<string>(type: "text", nullable: true),
                    is_banned = table.Column<bool>(type: "boolean", nullable: false),
                    accepts_guidelines = table.Column<bool>(type: "boolean", nullable: false),
                    is_anonymous = table.Column<bool>(type: "boolean", nullable: false),
                    roles = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "levels",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    record_created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    record_updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: true),
                    creator_name = table.Column<string>(type: "text", nullable: true),
                    game_modes = table.Column<List<string>>(type: "text[]", nullable: true),
                    workshop_id = table.Column<long>(type: "bigint", nullable: true),
                    creator_id = table.Column<long>(type: "bigint", nullable: true),
                    file_name = table.Column<string>(type: "text", nullable: true),
                    file_url = table.Column<string>(type: "text", nullable: true),
                    image_url = table.Column<string>(type: "text", nullable: true),
                    created_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    updated_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    workshop_tags = table.Column<List<string>>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_levels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<int>(type: "integer", nullable: false),
                    is_exclusive = table.Column<bool>(type: "boolean", nullable: false),
                    is_fallback = table.Column<bool>(type: "boolean", nullable: false),
                    is_auto_tag = table.Column<bool>(type: "boolean", nullable: false),
                    display_group_id = table.Column<long>(type: "bigint", nullable: false),
                    logic_group_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tags", x => x.id);
                    table.ForeignKey(
                        name: "fk_tags_display_groups_display_group_id",
                        column: x => x.display_group_id,
                        principalTable: "display_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_tags_logic_groups_logic_group_id",
                        column: x => x.logic_group_id,
                        principalTable: "logic_groups",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_bans",
                columns: table => new
                {
                    steam_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    banned_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    banner_id = table.Column<long>(type: "bigint", nullable: false),
                    reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_bans", x => x.steam_id);
                    table.ForeignKey(
                        name: "fk_user_bans_users_banner_id",
                        column: x => x.banner_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_bans_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_sessions",
                columns: table => new
                {
                    token = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    last_seen_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_sessions", x => x.token);
                    table.ForeignKey(
                        name: "fk_user_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "contributions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    level_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    auto_tagger_id = table.Column<long>(type: "bigint", nullable: true),
                    action = table.Column<int>(type: "integer", nullable: false),
                    contributed_on = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    is_reverted = table.Column<bool>(type: "boolean", nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contributions", x => x.id);
                    table.ForeignKey(
                        name: "fk_contributions_auto_taggers_auto_tagger_id",
                        column: x => x.auto_tagger_id,
                        principalTable: "auto_taggers",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_contributions_levels_level_id",
                        column: x => x.level_id,
                        principalTable: "levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contributions_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_contributions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "level_tags",
                columns: table => new
                {
                    level_id = table.Column<long>(type: "bigint", nullable: false),
                    tag_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_level_tags", x => new { x.level_id, x.tag_id });
                    table.ForeignKey(
                        name: "fk_level_tags_levels_level_id",
                        column: x => x.level_id,
                        principalTable: "levels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_level_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_auto_taggers_guid",
                table: "auto_taggers",
                column: "guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contributions_auto_tagger_id",
                table: "contributions",
                column: "auto_tagger_id");

            migrationBuilder.CreateIndex(
                name: "ix_contributions_level_id",
                table: "contributions",
                column: "level_id");

            migrationBuilder.CreateIndex(
                name: "ix_contributions_tag_id",
                table: "contributions",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_contributions_user_id",
                table: "contributions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_display_groups_name",
                table: "display_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_level_tags_tag_id",
                table: "level_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_levels_creator_id",
                table: "levels",
                column: "creator_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_logic_groups_name",
                table: "logic_groups",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tags_display_group_id",
                table: "tags",
                column: "display_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_logic_group_id",
                table: "tags",
                column: "logic_group_id");

            migrationBuilder.CreateIndex(
                name: "ix_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_bans_banner_id",
                table: "user_bans",
                column: "banner_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_bans_user_id",
                table: "user_bans",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_sessions_user_id",
                table: "user_sessions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_steam_id",
                table: "users",
                column: "steam_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "contributions");

            migrationBuilder.DropTable(
                name: "level_locks");

            migrationBuilder.DropTable(
                name: "level_tags");

            migrationBuilder.DropTable(
                name: "settings");

            migrationBuilder.DropTable(
                name: "user_bans");

            migrationBuilder.DropTable(
                name: "user_sessions");

            migrationBuilder.DropTable(
                name: "auto_taggers");

            migrationBuilder.DropTable(
                name: "levels");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "creators");

            migrationBuilder.DropTable(
                name: "display_groups");

            migrationBuilder.DropTable(
                name: "logic_groups");
        }
    }
}

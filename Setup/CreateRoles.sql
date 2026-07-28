-- Roles --

CREATE ROLE distags_user NOLOGIN;
GRANT CONNECT ON DATABASE distags TO distags_user;
GRANT USAGE ON SCHEMA public TO distags_user;

CREATE ROLE distags_admin NOLOGIN;
GRANT distags_user TO distags_admin;
GRANT CREATE ON SCHEMA public TO distags_admin;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO distags_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL PRIVILEGES ON TABLES TO distags_admin;

CREATE ROLE distags_read_levels NOLOGIN;
GRANT distags_user TO distags_read_levels;
GRANT SELECT ON TABLE levels, creators, level_tags, tags TO distags_read_levels;

CREATE ROLE distags_manage_levels NOLOGIN;
GRANT distags_user TO distags_manage_levels;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE levels, creators TO distags_manage_levels;

CREATE ROLE distags_use_level_locks NOLOGIN;
GRANT distags_user TO distags_use_level_locks;
GRANT SELECT, UPDATE ON TABLE level_locks TO distags_use_level_locks;

CREATE ROLE distags_manage_level_locks NOLOGIN;
GRANT distags_user TO distags_manage_level_locks;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE level_locks TO distags_manage_level_locks;

CREATE ROLE distags_read_tags NOLOGIN;
GRANT distags_user TO distags_read_tags;
GRANT SELECT ON TABLE display_groups, logic_groups, tags TO distags_read_tags;

CREATE ROLE distags_manage_tags NOLOGIN;
GRANT distags_user TO distags_manage_tags;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE display_groups, logic_groups, tags TO distags_manage_tags;
GRANT USAGE, SELECT, UPDATE ON SEQUENCE display_groups_id_seq, logic_groups_id_seq, tags_id_seq TO distags_manage_tags;

CREATE ROLE distags_assign_tags NOLOGIN;
GRANT distags_user TO distags_assign_tags;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE level_tags TO distags_assign_tags;

CREATE ROLE distags_manage_users NOLOGIN;
GRANT distags_user TO distags_manage_users;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE users, user_sessions TO distags_manage_users;

CREATE ROLE distags_manage_apps NOLOGIN;
GRANT distags_user TO distags_manage_apps;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE apps TO distags_manage_apps;
GRANT USAGE, SELECT, UPDATE ON SEQUENCE apps_id_seq TO distags_manage_apps;

CREATE ROLE distags_manage_contributions NOLOGIN;
GRANT distags_user TO distags_manage_contributions;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE contributions TO distags_manage_contributions;
GRANT USAGE, SELECT, UPDATE ON SEQUENCE contributions_id_seq TO distags_manage_contributions;

CREATE ROLE distags_manage_user_bans NOLOGIN;
GRANT distags_user TO distags_manage_user_bans;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE user_bans TO distags_manage_user_bans;

CREATE ROLE distags_manage_settings NOLOGIN;
GRANT distags_user TO distags_manage_settings;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE settings TO distags_manage_settings;

-- Users --

CREATE ROLE distags_migrator WITH LOGIN PASSWORD '';
GRANT distags_admin TO distags_migrator;

CREATE ROLE distags_api WITH LOGIN PASSWORD '';
GRANT distags_read_levels TO distags_api;
GRANT distags_use_level_locks TO distags_api;
GRANT distags_manage_tags TO distags_api;
GRANT distags_assign_tags TO distags_api;
GRANT distags_manage_users TO distags_api;
GRANT distags_manage_apps TO distags_api;
GRANT distags_manage_contributions TO distags_api;
GRANT distags_manage_user_bans TO distags_api;
GRANT distags_manage_settings TO distags_api;

CREATE ROLE distags_scraper WITH LOGIN PASSWORD '';
GRANT distags_manage_levels TO distags_scraper;
GRANT distags_manage_level_locks TO distags_scraper;
GRANT distags_read_tags TO distags_scraper;
GRANT distags_assign_tags TO distags_scraper;

CREATE ROLE distags_auto_tagger WITH LOGIN PASSWORD '';
GRANT distags_read_levels TO distags_auto_tagger;
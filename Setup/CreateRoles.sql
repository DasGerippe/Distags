-- Roles --

CREATE ROLE distance_level_tags_user NOLOGIN;
GRANT CONNECT ON DATABASE distance_level_tags TO distance_level_tags_user;
GRANT USAGE ON SCHEMA public TO distance_level_tags_user;

CREATE ROLE distance_level_tags_admin NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_admin;
GRANT CREATE ON SCHEMA public TO distance_level_tags_admin;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO distance_level_tags_admin;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL PRIVILEGES ON TABLES TO distance_level_tags_admin;

CREATE ROLE distance_level_tags_read_levels NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_read_levels;
GRANT SELECT ON TABLE levels, creators, level_tags, tags TO distance_level_tags_read_levels;

CREATE ROLE distance_level_tags_manage_levels NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_manage_levels;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE levels, creators TO distance_level_tags_manage_levels;

CREATE ROLE distance_level_tags_manage_tags NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_manage_tags;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE tag_groups, tags TO distance_level_tags_manage_tags;
GRANT USAGE, SELECT, UPDATE ON SEQUENCE tags_id_seq, tag_groups_id_seq TO distance_level_tags_manage_tags;

CREATE ROLE distance_level_tags_assign_tags NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_assign_tags;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE level_tags TO distance_level_tags_assign_tags;

CREATE ROLE distance_level_tags_manage_users NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_manage_users;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE users, user_sessions TO distance_level_tags_manage_users;

CREATE ROLE distance_level_tags_manage_api_keys NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_manage_api_keys;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE api_keys TO distance_level_tags_manage_api_keys;

CREATE ROLE distance_level_tags_manage_contributions NOLOGIN;
GRANT distance_level_tags_user TO distance_level_tags_manage_contributions;
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLE contributions TO distance_level_tags_manage_api_keys;

-- Users --

CREATE ROLE distance_level_tags_migrator WITH LOGIN PASSWORD '';
GRANT distance_level_tags_admin TO distance_level_tags_migrator;

CREATE ROLE distance_level_tags_api WITH LOGIN PASSWORD '';
GRANT distance_level_tags_read_levels TO distance_level_tags_api;
GRANT distance_level_tags_manage_tags TO distance_level_tags_api;
GRANT distance_level_tags_assign_tags TO distance_level_tags_api;
GRANT distance_level_tags_manage_users TO distance_level_tags_api;
GRANT distance_level_tags_manage_api_keys TO distance_level_tags_api;
GRANT distance_level_tags_manage_contributions TO distance_level_tags_api;

CREATE ROLE distance_level_tags_scraper WITH LOGIN PASSWORD '';
GRANT distance_level_tags_manage_levels TO distance_level_tags_scraper;

CREATE ROLE distance_level_tags_auto_tagger WITH LOGIN PASSWORD '';
GRANT distance_level_tags_read_levels TO distance_level_tags_auto_tagger;
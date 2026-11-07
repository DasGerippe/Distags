-- Creates the roles required by Distags applications:
--   Admin      {DBNAME}_admin      DDL role, used for migrations.
--   App        {DBNAME}_app        DML role, used by Distags applications reading and writing data.

-- Variables:
--   admin_password     required    Password of the Distags Admin role.
--   app_password       required    Password of the Distags App role.

-- Execution:
--   psql -v admin_password='' \
--        -v app_password='' \
--        -d distags \
--        -f create_roles.sql

\set ON_ERROR_STOP on

\if :{?admin_password} \else \echo 'ERROR: Variable admin_password is required.' \quit \endif
\if :{?app_password} \else \echo 'ERROR: Variable app_password is required.' \quit \endif

\set admin_role :DBNAME _admin
\set app_role :DBNAME _app

CREATE ROLE :"admin_role" LOGIN PASSWORD :'admin_password';
CREATE ROLE :"app_role"   LOGIN PASSWORD :'app_password';

REVOKE ALL ON DATABASE :"DBNAME" FROM PUBLIC;
REVOKE ALL ON SCHEMA public FROM PUBLIC;

ALTER DATABASE :"DBNAME" OWNER TO :"admin_role";
GRANT ALL ON SCHEMA public TO :"admin_role";

GRANT CONNECT ON DATABASE :"DBNAME" TO :"app_role";
GRANT USAGE ON SCHEMA public TO :"app_role";

ALTER DEFAULT PRIVILEGES FOR ROLE :"admin_role" IN SCHEMA public
    GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO :"app_role";
ALTER DEFAULT PRIVILEGES FOR ROLE :"admin_role" IN SCHEMA public
    GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO :"app_role";

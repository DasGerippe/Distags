#!/bin/bash
set -euo pipefail

admin_password="$(< /run/secrets/db_distags_admin_password)"
app_password="$(< /run/secrets/db_distags_app_password)"

psql -v ON_ERROR_STOP=1 \
     -v admin_password="$admin_password" \
     -v app_password="$app_password" \
     -U "$POSTGRES_USER" \
     -d "$POSTGRES_DB" \
     -f /opt/distags/sql/create-roles.sql

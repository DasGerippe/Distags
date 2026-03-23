# Distance Level Tagging

## Dev info

### Add/Remove database migrations

EF Core migrations are stored in the project DataStoringMigrations.
Databases can be migrated using the project DataStoringMigrator.

To add/remove a migration to/from DataStoringMigrations, run one of the following commands:

```bash
# Add a new migration.
dotnet ef migrations add <MigrationName> --project DataStoringMigrations --startup-project DataStoringMigrator

# Remove the last migration.
dotnet ef migrations remove --project DataStoringMigrations --startup-project DataStoringMigrator
```
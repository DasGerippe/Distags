# Distags

Distags is a collection of software that allows users to add (and remove) tags to levels of the game Distance by Refract. Levels can then be searched based on those tags.

## Dev info

### Add/Remove database migrations

EF Core migrations are stored in the project DataStoringMigrations.
Databases can be migrated using the project DataStoringMigrator.

To add/remove a migration to/from DataStoringMigrations, run one of the following commands:

```bash
# Add a new migration.
dotnet ef migrations add <MigrationName> --project Distags.Persistence.Dev --startup-project Distags.Persistence.Migrator

# Remove the last migration.
dotnet ef migrations remove --project Distags.Persistence.Dev --startup-project Distags.Persistence.Migrator
```
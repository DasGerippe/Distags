# Distags

Distags is a collection of software for tagging levels of the game Distance by Refract. Levels can then be searched based on those tags.

## Architecture Communication Canvas

![Architecture Communication Canvas](docs/ArchitectureCommunicationCanvas.svg)

## Dev info

### Add/Remove database migrations

EF Core migrations are stored in the project `Distags.Persistence.Migrations`.
Databases can be migrated using the project `Distags.Persistence.Migrator`.

To add/remove a migration to/from `Distags.Persistence.Migrations`, run one of the following commands:

```shell
# Add a new migration.
dotnet ef migrations add <MigrationName> --project Distags.Persistence.Migrations --startup-project Distags.Persistence.Migrator

# Remove the last migration.
dotnet ef migrations remove --project Distags.Persistence.Migrations --startup-project Distags.Persistence.Migrator
```

## Deployment

The software is deployed through docker images/containers.

Required steps:

- If docker images should be built on the hosting environment:
  - Clone the repository onto the hosting environment.
- If docker images will be built separately:
  - Build the docker images.
  - Export the images using `docker save` and import them on the host using `docker load`.
  - Copy `compose.yaml`, `.env.example`, and the `infra` directory onto the hosting server.
- Rename `.env.example` to `.env` and set the missing env variables inside it.
- Put all required secrets (API keys, passwords) into the corresponding secret files in `./infra/secrets`.
- Start the core application containers with `docker compose up -d`.
- Create a cronjob for periodically starting the distags-steam-data-scraper container.

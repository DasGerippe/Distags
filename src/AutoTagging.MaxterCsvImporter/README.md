# MaxterCsvImporter

AutoTagger that tags *Distance* levels based on Maxter Max' Google Sheet "Distance Map Tagging". (Exported and included in this project as `maxter_tagging_all_levels.csv`).

This AutoTagger needs a **mapping file** (`maxter_tag_mapping.json`) that translates the free-text values from the CSV file into actual Distags tag names. The mapping file can be generated/maintained by a small CLI mode of this same project.

## Running the MaxterCsvImporter

```shell
# (Re)generate the mapping file.
dotnet run --project AutoTagging.MaxterCsvImporter -- generate-mapping

# Run just this auto-tagger.
dotnet run --project AutoTaggerRunner -- MaxterCsvImporter
```

## Key Properties of the Mapping Generation

- Additive only. Existing keys are never overwritten or removed.
- Car Abilities target names are re-validated every run, not just seeded once. If e.g. `jump-disabled` gets renamed or removed in Distags, the next `generate-mapping` run un-maps it and warns, instead of silently pointing at a tag that no longer exists.
- Length/WR-length buckets (`LengthBuckets`/`WrLengthBuckets`) are only seeded with defaults **if empty**. Their thresholds/tag names can be edited freely afterwards.

## Tagging

Calculating the `TagDelta` for a level includes the following steps:

- Get a set of desired tags based on the CSV rows of that level and the mapping file.
- Calculate the diff between the desired tags and the current tags.
- Strip tags from `TagDelta.TagsToRemove` that weren't previously added by this AutoTagger.

### Special Cases

- **Car Abilities `Toggles`**:
  - The CSV only records *that* a level has a togglable ability, not *which* one. If a row's `Car Abilities` cell contains `Toggles`, none of the enabled/disabled ability tags are set for that level.
- **Length / WR Length bucketing**:
  - Cell values are parsed to minutes (according to the `sec`/`min`/`h` suffixes).
  - The **highest** value (if multiple) in a cell is matched with the first `lengthBuckets`/`wrLengthBuckets` entry where `maxMinutes >= value`.
- **Duplicate Workshop IDs** in the CSV are merged by first-non-empty-value-per-column.
- **`WR Route Style`** is parsed from the CSV but intentionally has no mapping section or tag output because there's no corresponding `DisplayGroup`/`LogicGroup` in Distags for it (yet).

with entry_counts as (
    select
        level_id,
        count(*) as entry_count
    from GAMEMODE_leaderboard_entries
    group by level_id
),
level_stats as (
    select
        level_id,
        entry_count,
        min(time) as world_record_time,
        percentile_cont(0.5) within group (order by time) as median_time,
        -- 60% Confidence Interval (20th to 80th percentile: z = 0.842)
        -- quantile offset = 0.5 * z / sqrt(count)
        percentile_cont(greatest(0, 0.5 - 0.421 / sqrt(entry_count))) within group (order by time) as ci_low_time,
        percentile_cont(   least(1, 0.5 + 0.421 / sqrt(entry_count))) within group (order by time) as ci_high_time
    from GAMEMODE_leaderboard_entries
    join entry_counts using (level_id)
    group by level_id, entry_count
)
select
    level.id,
    level.name,
    stats.entry_count,
    stats.world_record_time,
    stats.median_time,
    stats.ci_low_time,
    stats.ci_high_time
from level_stats stats
join levels level on level.id = stats.level_id

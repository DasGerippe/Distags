using AppHostUtils;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Reflection;

namespace AutoTagging.Length;

public class SeekrDbReader
{
    private readonly string _ConnectionString;

    private readonly string _QueryLevelStatsSql;

    public SeekrDbReader(IConfiguration configuration)
    {
        _ConnectionString = configuration.GetRequiredConnectionString("SeekrDatabase");

        _QueryLevelStatsSql = ReadQueryLevelStatsSqlFile();
    }

    private string ReadQueryLevelStatsSqlFile()
    {
        const string sqlFilePath = $"{nameof(AutoTagging)}.{nameof(Length)}.query_level_stats.sql";

        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sqlFilePath)
                              ?? throw new InvalidOperationException($"Embedded resource {sqlFilePath} not found.");
        using StreamReader streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }

    public async Task<IReadOnlyCollection<LevelStats>> QueryLevelStatsAsync(CancellationToken cancellationToken = default)
    {
        await using NpgsqlDataSource dataSource = NpgsqlDataSource.Create(_ConnectionString);

        Dictionary<long, LevelStats> levelStatsDictionary = [];

        await QueryLevelStatsAsync(dataSource, "sprint", levelStatsDictionary, cancellationToken).ConfigureAwait(false);
        await QueryLevelStatsAsync(dataSource, "challenge", levelStatsDictionary, cancellationToken).ConfigureAwait(false);

        return levelStatsDictionary.Values;
    }

    private async Task QueryLevelStatsAsync(
        NpgsqlDataSource dataSource,
        string gameMode,
        Dictionary<long, LevelStats> levelStatsDictionary,
        CancellationToken cancellationToken = default)
    {
        string sqlQuery = GetGameModeStatsQuery(gameMode);
        await using NpgsqlCommand sprintCommand = dataSource.CreateCommand(sqlQuery);
        NpgsqlDataReader reader = await sprintCommand.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            long levelId = reader.GetInt64(0);
            if (levelStatsDictionary.ContainsKey(levelId))
                continue;

            levelStatsDictionary[levelId] = new LevelStats
            {
                LevelId = levelId,
                LevelName = reader.GetString(1),
                EntryCount = reader.GetInt32(2),
                WorldRecordTime = TimeSpan.FromMilliseconds(reader.GetInt32(3)),
                MedianTime = TimeSpan.FromMilliseconds(reader.GetDouble(4)),
                CiLowTime = TimeSpan.FromMilliseconds(reader.GetDouble(5)),
                CiHighTime = TimeSpan.FromMilliseconds(reader.GetDouble(6)),
            };
        }
    }

    private string GetGameModeStatsQuery(string gameMode)
    {
        return _QueryLevelStatsSql.Replace("GAMEMODE", gameMode);
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System.Reflection;

namespace Distags.Persistence;

public static class MigrationBuilderExtensions
{
    public static void SqlEmbeddedFile(this MigrationBuilder migrationBuilder, string sqlFilePath)
    {
        sqlFilePath = $"{nameof(Distags)}.{nameof(Persistence)}.{sqlFilePath}";

        using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sqlFilePath)
                              ?? throw new InvalidOperationException($"Embedded resource {sqlFilePath} not found.");
        using StreamReader streamReader = new StreamReader(stream);

        string sql = streamReader.ReadToEnd();
        migrationBuilder.Sql(sql);
    }
}

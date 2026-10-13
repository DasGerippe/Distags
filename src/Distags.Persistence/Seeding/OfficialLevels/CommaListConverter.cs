using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Distags.Persistence.Seeding.OfficialLevels;

public sealed class CommaListConverter : ITypeConverter
{
    public object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return new List<string>();

        return text
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
    }

    public string? ConvertToString(object? value, IWriterRow row, MemberMapData memberMapData)
    {
        return value is List<string> list
            ? string.Join(',', list)
            : string.Empty;
    }
}

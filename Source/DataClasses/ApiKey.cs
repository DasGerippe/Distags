namespace DataClasses;

public class ApiKey
{
    public long Id { get; set; }

    public string Value { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;
}
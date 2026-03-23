namespace LevelTagApi.Dtos;

public class ApiKeyDto
{
    public required string ApiKey { get; set; }

    public string Comment { get; set; } = string.Empty;
}
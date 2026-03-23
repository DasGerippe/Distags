namespace DataClasses;

public class User
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public List<string> Roles { get; set; } = [];
}
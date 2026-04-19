namespace LevelTagApi.Dtos.Contributions;

public class ContributionDto
{
    public required long Id { get; set; }

    public required LevelDto? Level { get; set; }

    public required List<ChangeDto> Changes { get; set; }

    public required UserDto? User { get; set; }

    public required AppDto? App { get; set; }

    public required DateTimeOffset ContributedOn { get; set; }
}
namespace Distags.Contracts;

public class ContributionDto
{
    public required long Id { get; set; }

    public required LevelDto Level { get; set; }

    public required List<TagChangeDto> Changes { get; set; }

    public required ContributionUserDto? User { get; set; }

    public required ContributionAppDto? App { get; set; }

    public required DateTimeOffset ContributedOn { get; set; }
}

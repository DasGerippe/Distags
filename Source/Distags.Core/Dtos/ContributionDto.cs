namespace Distags.Core.Dtos;

public class ContributionDto
{

    public required long Id { get; set; }

    public required ContributionLevelDto? Level { get; set; }

    public required List<ContributionChangeDto> Changes { get; set; }

    public required ContributionUserDto? User { get; set; }

    public required ContributionAppDto? App { get; set; }

    public required DateTimeOffset ContributedOn { get; set; }
}
namespace LevelTagApi.Dtos;

public class ContributionDto
{
    public long Id { get; set; }

    public required LevelDto Level { get; set; }

    public required TagDto Tag { get; set; }

    public required ContributorDto Contributor { get; set; }

    public required ContributionAction Action { get; set; }

    public required DateTimeOffset ContributedOn { get; set; }
}
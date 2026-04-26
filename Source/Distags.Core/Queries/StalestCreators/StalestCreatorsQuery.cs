namespace Distags.Core.Queries.StalestCreators;

public class StalestCreatorsQuery : IQuery<List<long>>
{
    public int? Limit { get; set; }
}
namespace Distags.Core.Exceptions;

public class IncompleteEntityAggregateException : DistagsException
{
    private readonly string? _AggregateName;

    public override string Message => string.IsNullOrEmpty(_AggregateName)
        ? base.Message
        : $"{base.Message}{Environment.NewLine}Aggregate name: {_AggregateName}";

    public IncompleteEntityAggregateException()
    {
    }

    public IncompleteEntityAggregateException(string? message) : base(message)
    {
    }

    public IncompleteEntityAggregateException(string? message, string? aggregateName) : base(message)
    {
        _AggregateName = aggregateName;
    }
}
using System.Diagnostics.CodeAnalysis;

namespace Distags.Core;

public class DomainResult
{
    [MemberNotNullWhen(false, nameof(Error))]
    public virtual bool IsSuccess { get; }

    public DomainError? Error { get; }

    protected DomainResult()
    {
        IsSuccess = true;
    }

    protected DomainResult(DomainError error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static DomainResult Success()
    {
        return new DomainResult();
    }

    public static DomainResult Failure(DomainError error)
    {
        return new DomainResult(error);
    }
}

public class DomainResult<TValue> : DomainResult
{
    [MemberNotNullWhen(true, nameof(Value))]
    public override bool IsSuccess => base.IsSuccess;

    public TValue? Value { get; }

    protected DomainResult(TValue value) : base()
    {
        Value = value;
    }

    protected DomainResult(DomainError error) : base(error)
    {
    }

    public static DomainResult<TValue> Success(TValue value)
    {
        return new DomainResult<TValue>(value);
    }

    public new static DomainResult<TValue> Failure(DomainError error)
    {
        return new DomainResult<TValue>(error);
    }
}

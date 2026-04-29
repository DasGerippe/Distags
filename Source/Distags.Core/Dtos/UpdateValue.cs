namespace Distags.Core.Dtos;

public readonly struct UpdateValue<TValue>
{
    public TValue Value { get; }

    public bool HasValue { get; }

    public UpdateValue(TValue value, bool hasValue)
    {
        Value = value;
        HasValue = hasValue;
    }

    public static implicit operator UpdateValue<TValue?>(TValue? value)
    {
        return new UpdateValue<TValue?>(value, true);
    }

    public static implicit operator TValue(UpdateValue<TValue> updateValue)
    {
        return updateValue.Value;
    }
}
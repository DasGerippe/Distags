namespace Distags.Api.Contracts;

public class PatchValue<TValue>
{
    public TValue? Value { get; init; }

    public static implicit operator PatchValue<TValue?>(TValue? value)
    {
        return new PatchValue<TValue?> { Value = value };
    }

    public static explicit operator TValue?(PatchValue<TValue> patchValue)
    {
        return patchValue.Value;
    }
}

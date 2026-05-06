namespace Distags.Api.Contracts;

public class PatchValue<TValue>
{
    public TValue? Value { get; set; }

    public static implicit operator TValue?(PatchValue<TValue> patchValue)
    {
        return patchValue.Value;
    }

    public static implicit operator PatchValue<TValue?>(TValue? value)
    {
        return new PatchValue<TValue?> { Value = value };
    }
}
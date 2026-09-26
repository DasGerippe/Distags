namespace Distags.Api.Contracts.Users;

public class PatchUserSettingsRequest
{
    public PatchValue<bool>? AcceptsGuidelines { get; init; }

    public PatchValue<bool>? IsAnonymous { get; init; }
}

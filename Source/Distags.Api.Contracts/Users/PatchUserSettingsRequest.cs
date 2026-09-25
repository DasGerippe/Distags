namespace Distags.Api.Contracts.Users;

public class PatchUserSettingsRequest
{
    public PatchValue<bool>? AcceptsGuidelines { get; set; }

    public PatchValue<bool>? IsAnonymous { get; set; }
}

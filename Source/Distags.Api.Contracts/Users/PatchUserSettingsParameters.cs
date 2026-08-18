namespace Distags.Api.Contracts.Users;

public class PatchUserSettingsParameters
{
    public PatchValue<bool>? AcceptsGuidelines { get; set; }

    public PatchValue<bool>? IsAnonymous { get; set; }
}

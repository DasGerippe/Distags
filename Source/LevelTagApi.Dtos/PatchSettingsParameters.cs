namespace LevelTagApi.Dtos;

public class PatchSettingsParameters
{
    public PatchValue<bool>? AcceptsGuidelines { get; set; }

    public PatchValue<bool>? IsAnonymous { get; set; }
}
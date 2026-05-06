namespace Distags.Core.Commands.UpdateUserSettings;

public class UpdateUserSettingsCommand : ICommand
{
    public long SteamId { get; set; }

    public UpdateValue<bool> AcceptsGuidelines { get; set; }

    public UpdateValue<bool> IsAnonymous { get; set; }
}
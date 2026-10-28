namespace Distags.Core.Commands.CreateOrUpdateAutoTagger;

public class CreateOrUpdateAutoTaggerCommand : ICommand
{
    public required Guid Guid { get; set; }

    public UpdateValue<string> Name { get; set; }

    public UpdateValue<DateTimeOffset> LastRunOn { get; set; }

    public UpdateValue<string?> State { get; set; }
}

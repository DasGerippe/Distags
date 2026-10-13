namespace Distags.Api.Contracts.Configuration;

public class PatchConfigurationRequest
{
    public PatchValue<bool>? MaintenanceActive { get; init; }
}

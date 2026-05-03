using Distags.Core.Dtos;

namespace Distags.Core.Queries.AppByApiKey;

public class AppByApiKeyQuery : IQuery<AppDto?>
{
    public string ApiKey { get; set; } = string.Empty;
}
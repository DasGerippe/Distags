using Distags.Core.Dtos;

namespace Distags.Core.Queries.AppByName;

public class AppByNameQuery : IQuery<AppDto>
{
    public string AppName { get; set; } = string.Empty;
}
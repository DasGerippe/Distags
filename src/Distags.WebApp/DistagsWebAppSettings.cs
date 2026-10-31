using System.ComponentModel.DataAnnotations;

namespace Distags.WebApp;

public class DistagsWebAppSettings
{
    [Url]
    public string DistagsApiUrlInternal { get; init; } = string.Empty;

    [Url]
    public string DistagsApiUrlExternal { get; init; } = string.Empty;
}

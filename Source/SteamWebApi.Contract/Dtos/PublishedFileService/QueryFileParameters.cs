using HttpUtils;

namespace SteamWebApi.Dtos.PublishedFileService;

public class QueryFileParameters
{
    [QueryParameter("key")]
    public string? Key { get; set; }

    [QueryParameter("appid")]
    public int? AppId { get; set; }

    [QueryParameter("query_type")]
    public QueryType? QueryType { get; set; }

    [QueryParameter("numperpage")]
    public int? ItemsPerPage { get; set; }

    [QueryParameter("return_metadata")]
    public bool? ReturnMetadata { get; set; }

    [QueryParameter("cursor")]
    public string? Cursor { get; set; } = "*";
}
using HttpUtils;

namespace SteamWebApi.Dtos.PublishedFileService;

public class QueryFileParameters
{
    [HttpParameter("key")]
    public string? Key { get; set; }

    [HttpParameter("appid")]
    public int? AppId { get; set; }

    [HttpParameter("filetype")]
    public FileType? FileType { get; set; }

    [HttpParameter("query_type")]
    public QueryType? QueryType { get; set; }

    [HttpParameter("numperpage")]
    public int? ItemsPerPage { get; set; }

    [HttpParameter("return_metadata")]
    public bool? ReturnMetadata { get; set; }

    [HttpParameter("cursor")]
    public string? Cursor { get; set; } = "*";
}

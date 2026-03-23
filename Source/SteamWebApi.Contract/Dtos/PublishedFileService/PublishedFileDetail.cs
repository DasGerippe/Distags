using SteamWebApi.Converter;
using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.PublishedFileService;

public class PublishedFileDetail
{
    //[JsonPropertyName("result")]
    //public int Result { get; set; }
    
    [JsonPropertyName("publishedfileid")]
    public string? PublishedFileId { get; set; }
    
    [JsonPropertyName("creator")]
    public string? CreatorId { get; set; }
    
    //[JsonPropertyName("creator_appid")]
    //public int CreatorAppId { get; set; }
    
    //[JsonPropertyName("consumer_appid")]
    //public int ConsumerAppId { get; set; }
    
    //[JsonPropertyName("consumer_shortcutid")]
    //public int ConsumerShortcutId { get; set; }
    
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }
    
    //[JsonPropertyName("file_size")]
    //public string FileSize { get; set; }
    
    //[JsonPropertyName("preview_file_size")]
    //public string PreviewFileSize { get; set; }
    
    [JsonPropertyName("file_url")]
    public string? FileUrl { get; set; }
    
    [JsonPropertyName("preview_url")]
    public string? PreviewUrl { get; set; }
    
    //[JsonPropertyName("url")]
    //public string Url { get; set; }
    
    //[JsonPropertyName("hcontent_file")]
    //public string HcontentFile { get; set; }
    
    //[JsonPropertyName("hcontent_preview")]
    //public string HcontentPreview { get; set; }
    
    [JsonPropertyName("title")]
    public string? Title { get; set; }
    
    //[JsonPropertyName("file_description")]
    //public string FileDescription { get; set; }
    
    [JsonPropertyName("time_created")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? TimeCreated { get; set; }
    
    [JsonPropertyName("time_updated")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public DateTime? TimeUpdated { get; set; }
    
    //[JsonPropertyName("visibility")]
    //public int Visibility { get; set; }
    
    //[JsonPropertyName("flags")]
    //public int Flags { get; set; }
    
    //[JsonPropertyName("workshop_file")]
    //public bool WorkshopFile { get; set; }
    
    //[JsonPropertyName("workshop_accepted")]
    //public bool WorkshopAccepted { get; set; }
    
    //[JsonPropertyName("show_subscribe_all")]
    //public bool ShowSubscribeAll { get; set; }
    
    //[JsonPropertyName("num_comments_public")]
    //public int NumCommentsPublic { get; set; }
    
    //[JsonPropertyName("banned")]
    //public bool Banned { get; set; }
    
    //[JsonPropertyName("ban_reason")]
    //public string BanReason { get; set; }
    
    //[JsonPropertyName("banner")]
    //public string Banner { get; set; }
    
    //[JsonPropertyName("can_be_deleted")]
    //public bool CanBeDeleted { get; set; }
    
    //[JsonPropertyName("app_name")]
    //public string AppName { get; set; }
    
    //[JsonPropertyName("file_type")]
    //public int FileType { get; set; }
    
    //[JsonPropertyName("can_subscribe")]
    //public bool CanSubscribe { get; set; }
    
    //[JsonPropertyName("subscriptions")]
    //public int Subscriptions { get; set; }
    
    //[JsonPropertyName("favorited")]
    //public int Favorited { get; set; }
    
    //[JsonPropertyName("followers")]
    //public int Followers { get; set; }
    
    //[JsonPropertyName("lifetime_subscriptions")]
    //public int LifetimeSubscriptions { get; set; }
    
    //[JsonPropertyName("lifetime_favorited")]
    //public int LifetimeFavorited { get; set; }

    //[JsonPropertyName("lifetime_followers")]
    //public int LifetimeFollowers { get; set; }

    //[JsonPropertyName("lifetime_playtime")]
    //public string LifetimePlaytime { get; set; }

    //[JsonPropertyName("lifetime_playtime_sessions")]
    //public string LifetimePlaytimeSessions { get; set; }

    //[JsonPropertyName("views")]
    //public int Views { get; set; }

    //[JsonPropertyName("num_children")]
    //public int NumChildren { get; set; }

    //[JsonPropertyName("num_reports")]
    //public int NumReports { get; set; }

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = [];

    //[JsonPropertyName("language")]
    //public int Language { get; set; }
    
    //[JsonPropertyName("maybe_inappropriate_sex")]
    //public bool MaybeInappropriateSex { get; set; }
    
    //[JsonPropertyName("maybe_inappropriate_violence")]
    //public bool MaybeInappropriateViolence { get; set; }
    
    //[JsonPropertyName("revision_change_number")]
    //public string RevisionChangeNumber { get; set; }
    
    //[JsonPropertyName("revision")]
    //public int Revision { get; set; }
    
    //[JsonPropertyName("ban_text_check_result")]
    //public int BanTextCheckResult { get; set; }
}
using SteamWebApi.Converter;
using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.PublishedFileService;

public class PublishedFileDetail
{
    //[JsonPropertyName("result")]
    //public required int Result { get; set; }
    
    [JsonPropertyName("publishedfileid")]
    public required string PublishedFileId { get; set; }
    
    [JsonPropertyName("creator")]
    public required string CreatorId { get; set; }
    
    //[JsonPropertyName("creator_appid")]
    //public required int CreatorAppId { get; set; }
    
    //[JsonPropertyName("consumer_appid")]
    //public required int ConsumerAppId { get; set; }
    
    //[JsonPropertyName("consumer_shortcutid")]
    //public required int ConsumerShortcutId { get; set; }
    
    [JsonPropertyName("filename")]
    public required string Filename { get; set; }
    
    //[JsonPropertyName("file_size")]
    //public required string FileSize { get; set; }
    
    //[JsonPropertyName("preview_file_size")]
    //public required string PreviewFileSize { get; set; }
    
    [JsonPropertyName("file_url")]
    public required string FileUrl { get; set; }
    
    [JsonPropertyName("preview_url")]
    public required string PreviewUrl { get; set; }
    
    //[JsonPropertyName("url")]
    //public required string Url { get; set; }
    
    //[JsonPropertyName("hcontent_file")]
    //public required string HcontentFile { get; set; }
    
    //[JsonPropertyName("hcontent_preview")]
    //public required string HcontentPreview { get; set; }
    
    [JsonPropertyName("title")]
    public required string Title { get; set; }
    
    //[JsonPropertyName("file_description")]
    //public required string FileDescription { get; set; }
    
    [JsonPropertyName("time_created")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public required DateTimeOffset TimeCreated { get; set; }
    
    [JsonPropertyName("time_updated")]
    [JsonConverter(typeof(UnixDateTimeConverter))]
    public required DateTimeOffset TimeUpdated { get; set; }
    
    //[JsonPropertyName("visibility")]
    //public required int Visibility { get; set; }
    
    //[JsonPropertyName("flags")]
    //public required int Flags { get; set; }
    
    //[JsonPropertyName("workshop_file")]
    //public required bool WorkshopFile { get; set; }
    
    //[JsonPropertyName("workshop_accepted")]
    //public required bool WorkshopAccepted { get; set; }
    
    //[JsonPropertyName("show_subscribe_all")]
    //public required bool ShowSubscribeAll { get; set; }
    
    //[JsonPropertyName("num_comments_public")]
    //public required int NumCommentsPublic { get; set; }
    
    //[JsonPropertyName("banned")]
    //public required bool Banned { get; set; }
    
    //[JsonPropertyName("ban_reason")]
    //public required string BanReason { get; set; }
    
    //[JsonPropertyName("banner")]
    //public required string Banner { get; set; }
    
    //[JsonPropertyName("can_be_deleted")]
    //public required bool CanBeDeleted { get; set; }
    
    //[JsonPropertyName("app_name")]
    //public required string AppName { get; set; }
    
    //[JsonPropertyName("file_type")]
    //public required int FileType { get; set; }
    
    //[JsonPropertyName("can_subscribe")]
    //public required bool CanSubscribe { get; set; }
    
    //[JsonPropertyName("subscriptions")]
    //public required int Subscriptions { get; set; }
    
    //[JsonPropertyName("favorited")]
    //public required int Favorited { get; set; }
    
    //[JsonPropertyName("followers")]
    //public required int Followers { get; set; }
    
    //[JsonPropertyName("lifetime_subscriptions")]
    //public required int LifetimeSubscriptions { get; set; }
    
    //[JsonPropertyName("lifetime_favorited")]
    //public required int LifetimeFavorited { get; set; }

    //[JsonPropertyName("lifetime_followers")]
    //public required int LifetimeFollowers { get; set; }

    //[JsonPropertyName("lifetime_playtime")]
    //public required string LifetimePlaytime { get; set; }

    //[JsonPropertyName("lifetime_playtime_sessions")]
    //public required string LifetimePlaytimeSessions { get; set; }

    //[JsonPropertyName("views")]
    //public required int Views { get; set; }

    //[JsonPropertyName("num_children")]
    //public required int NumChildren { get; set; }

    //[JsonPropertyName("num_reports")]
    //public required int NumReports { get; set; }

    [JsonPropertyName("tags")]
    public required List<Tag> Tags { get; set; }

    //[JsonPropertyName("language")]
    //public required int Language { get; set; }
    
    //[JsonPropertyName("maybe_inappropriate_sex")]
    //public required bool MaybeInappropriateSex { get; set; }
    
    //[JsonPropertyName("maybe_inappropriate_violence")]
    //public required bool MaybeInappropriateViolence { get; set; }
    
    //[JsonPropertyName("revision_change_number")]
    //public required string RevisionChangeNumber { get; set; }
    
    //[JsonPropertyName("revision")]
    //public required int Revision { get; set; }
    
    //[JsonPropertyName("ban_text_check_result")]
    //public required int BanTextCheckResult { get; set; }
}
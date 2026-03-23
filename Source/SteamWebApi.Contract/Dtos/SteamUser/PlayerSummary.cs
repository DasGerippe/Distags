using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.SteamUser;

public class PlayerSummary
{

    [JsonPropertyName("steamid")]
    public required string SteamId { get; set; }

    //[JsonPropertyName("communityvisibilitystate")]
    //public required int CommunityVisibilityState { get; set; }

    //[JsonPropertyName("profilestate")]
    //public required int ProfileState { get; set; }

    [JsonPropertyName("personaname")]
    public required string PersonaName { get; set; }

    //[JsonPropertyName("commentpermission")]
    //public required int CommentPermission { get; set; }

    //[JsonPropertyName("profileurl")]
    //public required string ProfileUrl { get; set; }

    //[JsonPropertyName("avatar")]
    //public required string AvatarSmallUrl { get; set; }

    //[JsonPropertyName("avatarmedium")]
    //public required string AvatarMediumUrl { get; set; }

    //[JsonPropertyName("avatarfull")]
    //public required string AvatarFullUrl { get; set; }

    [JsonPropertyName("avatarhash")]
    public required string AvatarHash { get; set; }

    //[JsonPropertyName("lastlogoff")]
    //public required int LastLogOff { get; set; }

    //[JsonPropertyName("personastate")]
    //public required int PersonaState { get; set; }
    
    //[JsonPropertyName("realname")]
    //public required string RealName { get; set; }
    
    //[JsonPropertyName("primaryclanid")]
    //public required string PrimaryClanId { get; set; }
    
    //[JsonPropertyName("timecreated")]
    //public required int TimeCreated { get; set; }
    
    //[JsonPropertyName("personastateflags")]
    //public required int PersonaStateFlags { get; set; }
    
    //[JsonPropertyName("loccountrycode")]
    //public required string LocationCountryCode { get; set; }
}
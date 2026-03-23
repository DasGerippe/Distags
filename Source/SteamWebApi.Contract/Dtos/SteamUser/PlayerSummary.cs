using System.Text.Json.Serialization;

namespace SteamWebApi.Dtos.SteamUser;

public class PlayerSummary
{

    [JsonPropertyName("steamid")]
    public string SteamId { get; set; }

    //[JsonPropertyName("communityvisibilitystate")]
    //public int CommunityVisibilityState { get; set; }

    //[JsonPropertyName("profilestate")]
    //public int ProfileState { get; set; }

    [JsonPropertyName("personaname")]
    public string PersonaName { get; set; }

    //[JsonPropertyName("commentpermission")]
    //public int CommentPermission { get; set; }

    //[JsonPropertyName("profileurl")]
    //public string ProfileUrl { get; set; }

    [JsonPropertyName("avatar")]
    public string AvatarUrl { get; set; }

    [JsonPropertyName("avatarmedium")]
    public string AvatarMediumUrl { get; set; }

    [JsonPropertyName("avatarfull")]
    public string AvatarFullUrl { get; set; }

    //[JsonPropertyName("avatarhash")]
    //public string AvatarHash { get; set; }

    //[JsonPropertyName("lastlogoff")]
    //public int LastLogOff { get; set; }

    //[JsonPropertyName("personastate")]
    //public int PersonaState { get; set; }
    
    //[JsonPropertyName("realname")]
    //public string RealName { get; set; }
    
    //[JsonPropertyName("primaryclanid")]
    //public string PrimaryClanId { get; set; }
    
    //[JsonPropertyName("timecreated")]
    //public int TimeCreated { get; set; }
    
    //[JsonPropertyName("personastateflags")]
    //public int PersonaStateFlags { get; set; }
    
    //[JsonPropertyName("loccountrycode")]
    //public string LocationCountryCode { get; set; }
}
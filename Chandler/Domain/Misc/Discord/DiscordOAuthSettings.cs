using Newtonsoft.Json;

namespace Domain.Misc.Discord
{
    /// <summary>
    /// Settings for discord oauth
    /// </summary>
    public class DiscordOAuthSettings
    {
        /// <summary>
        /// Application Client ID
        /// </summary>
        [JsonProperty("clientid")]
        public ulong? ClientId { get; set; } = null;

        /// <summary>
        /// Application Client Secret
        /// </summary>
        [JsonProperty("clientsecret")]
        public string ClientSecret { get; set; } = null;

        /// <summary>
        /// Application Redirect URI
        /// </summary>
        [JsonProperty("redirecturi")]
        public string RedirectUri { get; set; } = null;
    }
}

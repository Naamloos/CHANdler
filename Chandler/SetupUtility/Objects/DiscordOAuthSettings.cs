using Newtonsoft.Json;

namespace SetupUtility.Entities
{
    public class DiscordOAuthSettings
    {
        [JsonProperty("clientid")]
        public ulong? ClientId { get; set; } = null;

        [JsonProperty("clientsecret")]
        public string ClientSecret { get; set; } = null;

        [JsonProperty("redirecturi")]
        public string RedirectUri { get; set; } = null;
    }
}

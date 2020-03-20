using Newtonsoft.Json;

namespace Chandler.Data.Entities
{
    public class DiscordWebhookBody
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("embed")]
        public Embed Embed { get; set; }
    }
}

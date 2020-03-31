using Newtonsoft.Json;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Webhook Body Object
    /// </summary>
    public class DiscordWebhookBody
    {
        /// <summary>
        /// Message content
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Message Embed
        /// </summary>
        [JsonProperty("embed")]
        public Embed Embed { get; set; }
    }
}

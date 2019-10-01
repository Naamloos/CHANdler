using Newtonsoft.Json;

namespace Chandler.Data.Entities
{
    public class DiscordWebhookBody
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        //TODO: Add Embed Support
    }
}

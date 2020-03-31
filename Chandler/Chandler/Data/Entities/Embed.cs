using Newtonsoft.Json;
using System;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Discord Embed Object
    /// </summary>
    public class Embed
    {
        /// <summary>
        /// Embed Title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Embed Type
        /// </summary>
        [JsonProperty("type")]
        public string Type { get => "rich"; }

        /// <summary>
        /// Embed Description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Embed Url
        /// </summary>
        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        /// Embed Timestamp
        /// </summary>
        [JsonProperty("timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? Timestamp { get; set; }

        /// <summary>
        /// Embed Colour
        /// </summary>
        [JsonProperty("color", NullValueHandling = NullValueHandling.Ignore)]
        public int? Colour { get; set; }
    }
}

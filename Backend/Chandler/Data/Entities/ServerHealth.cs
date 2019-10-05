using Newtonsoft.Json;
using System;
using System.Text.Json.Serialization;

namespace Chandler.Data.Entities
{
    public class ServerHealth
    {
        [JsonProperty("overall_status")]
        public string OverallStatus { get; set; }

        [JsonProperty("database")]
        public bool DatabaseOk { get; set; }

        [JsonProperty("output")]
        public string Output { get; set; }

        [JsonProperty("latency")]
        public long Latency { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("uptime")]
        public TimeSpan Uptime { get; set; }
    }
}

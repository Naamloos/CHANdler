using System;
using Newtonsoft.Json;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Server Health Object
    /// </summary>
    public class ServerHealth
    {
        /// <summary>
        /// Overall status (Pass or Fail)
        /// </summary>
        [JsonProperty("overall_status")]
        public string OverallStatus { get; set; }

        /// <summary>
        /// If the database is connected to or not
        /// </summary>
        [JsonProperty("database")]
        public bool DatabaseOk { get; set; }

        /// <summary>
        /// The output message for errors
        /// </summary>
        [JsonProperty("output")]
        public string Output { get; set; }

        /// <summary>
        /// Latency on this request
        /// </summary>
        [JsonProperty("latency")]
        public long Latency { get; set; }

        /// <summary>
        /// Server Version
        /// </summary>
        [JsonProperty("version")]
        public int Version { get; set; }

        /// <summary>
        /// Total Uptime of the server
        /// </summary>
        [JsonProperty("uptime")]
        public TimeSpan Uptime { get; set; }
    }
}

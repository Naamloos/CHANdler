using Chandler.Data.Entities;
using Newtonsoft.Json;

namespace Chandler
{
    /// <summary>
    /// Server Configuration Object
    /// </summary>
    public class ServerConfig
    {
        /// <summary>
        /// Database Provider
        /// </summary>
        [JsonProperty("dbprovider")]
        public DatabaseProvider Provider { get; private set; }

        /// <summary>
        /// Connection string for the database
        /// </summary>
        [JsonProperty("dbstring")]
        public string ConnectionString { get; private set; }

        /// <summary>
        /// The base server address this application is running on
        /// </summary>
        [JsonProperty("server")]
        public string Server { get; private set; }

        /// <summary>
        /// Name for the entire site
        /// </summary>
        [JsonProperty("sitename")]
        public string SiteName { get; private set; }

        /// <summary>
        /// The logo for the site
        /// </summary>
        [JsonProperty("sitelogo")]
        public string SiteLogo { get; private set; }

        /// <summary>
        /// The default master password for deleting threads
        /// </summary>
        [JsonProperty("defaultpass")]
        public string DefaultPassword { get; private set; }
    }
}
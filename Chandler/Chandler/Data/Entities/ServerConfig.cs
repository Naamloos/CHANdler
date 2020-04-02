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
        public DatabaseProvider Provider { get; private set; } = DatabaseProvider.Sqlite;

        /// <summary>
        /// Connection string for the database
        /// </summary>
        [JsonProperty("dbstring")]
        public string ConnectionString { get; private set; } = "Data Source = Chandler.db";

        /// <summary>
        /// Name for the entire site
        /// </summary>
        [JsonProperty("sitename")]
        public string SiteName { get; private set; } = "CHANdler";

        /// <summary>
        /// The logo for the site
        /// </summary>
        [JsonProperty("sitelogo")]
        public string SiteLogo { get; private set; } = "/res/logo.jpg";

        /// <summary>
        /// The default master password for deleting threads
        /// </summary>
        [JsonProperty("defaultpass")]
        public string DefaultPassword { get; private set; } = "admin";

        /// <summary>
        /// Required for some metadata
        /// </summary>
        [JsonProperty("baseurl")]
        public string BaseUrl { get; private set; } = "";
    }
}
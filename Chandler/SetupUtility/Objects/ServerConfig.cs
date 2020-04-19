using Chandler.Data.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SetupUtility.Entities
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
        public DatabaseProvider Provider { get; set; } = DatabaseProvider.Sqlite;

        /// <summary>
        /// Connection string for the database
        /// </summary>
        [JsonProperty("dbstring")]
        public string ConnectionString { get; set; } = "Data Source = Chandler.db";

        /// <summary>
        /// Configuration for the site
        /// </summary>
        [JsonProperty("site")]
        public SiteConfig SiteConfig { get; set; } = new SiteConfig();

        /// <summary>
        /// Default admin users for the server
        /// </summary>
        [JsonProperty("defaultadmins")]
        public IEnumerable<DefaultAdmin> DefaultAdminUsers { get; set; } = new[]
        {
            new DefaultAdmin()
            {
                Email = "Admin@Admin.com",
                Username = "Admin",
                Password = "default_password"
            }
        };

        /// <summary>
        /// Configuration for discord oauth
        /// </summary>
        [JsonProperty("discordoauth")]
        public DiscordOAuthSettings DiscordOAuthSettings { get; set; } = null;
    }
}
using Chandler.Data.Entities;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

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
        /// The default master password for threads and admin user
        /// </summary>
        [JsonProperty("defaultpass")]
        public string DefaultPassword { get; private set; } = "default_password";

        /// <summary>
        /// Base url of the server the application is running on
        /// </summary>
        [JsonProperty("baseurl")]
        public string BaseUrl { get; private set; } = "http://localhost:2729";

        /// <summary>
        /// Default admin users for the server
        /// </summary>
        [JsonProperty("defaultadmins")]
        public IEnumerable<DefaultAdmin> DefaultAdminUsers { get; private set; } = new[]
        {
            new DefaultAdmin()
            {
                Email = "Admin@Admin.com",
                Username = "Admin",
                Password = "default_password"
            }
        };
    }
}
using Newtonsoft.Json;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Site Config Object
    /// </summary>
    public class SiteConfig
    {
        /// <summary>
        /// Name for the site
        /// </summary>
        [JsonProperty("sitename")]
        public string SiteName { get; set; } = "CHANdler";

        /// <summary>
        /// The favicon for the stie
        /// </summary>
        [JsonProperty("sitefav")]
        public string SiteFavicon { get; set; } = "/res/favicon.jpg";

        /// <summary>
        /// The logo for the site
        /// </summary>
        [JsonProperty("sitelogo")]
        public string SiteLogo { get; set; } = "/res/logo.jpg";

        /// <summary>
        /// Username for server admin
        /// </summary>
        [JsonProperty("adminusername")]
        public string AdminUsername { get; set; } = "admin";

        /// <summary>
        /// Username for server admin
        /// </summary>
        [JsonProperty("adminemail")]
        public string AdminEmail { get; set; } = "default@admin.com";

        /// <summary>
        /// Username for server admin
        /// </summary>
        [JsonProperty("adminpassword")]
        public string AdminPassword { get; set; } = "default_admin_password";

        /// <summary>
        /// The default master password for threads and admin user
        /// </summary>
        [JsonProperty("defaultpass")]
        public string DefaultPassword { get; set; } = "default_password";

        /// <summary>
        /// Base url of the server the application is running on
        /// </summary>
        [JsonProperty("baseurl")]
        public string BaseUrl { get; set; } = "http://localhost:2729";
    }
}

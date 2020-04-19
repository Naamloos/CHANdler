using Newtonsoft.Json;

namespace SetupUtility.Entities
{
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

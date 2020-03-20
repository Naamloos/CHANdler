using Chandler.Data;
using Newtonsoft.Json;

namespace Chandler
{
    public class ServerConfig
    {
        [JsonProperty("dbprovider")]
        public DatabaseProvider Provider { get; private set; }

        [JsonProperty("dbstring")]
        public string ConnectionString { get; private set; }

        [JsonProperty("server")]
        public string Server { get; private set; }

        [JsonProperty("sitename")]
        public string SiteName { get; private set; }

        [JsonProperty("sitelogo")]
        public string SiteLogo { get; private set; }

        [JsonProperty("defaultpass")]
        public string DefaultPassword { get; private set; }
    }
}
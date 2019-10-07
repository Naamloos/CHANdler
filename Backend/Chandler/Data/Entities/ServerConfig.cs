using Chandler.Data;
using Newtonsoft.Json;

namespace Chandler
{
    public struct ServerConfig
    {
        [JsonProperty("dbprovider")]
        public DatabaseProvider Provider { get; private set; }

        [JsonProperty("dbstring")]
        public string ConnectionString { get; private set; }
    }
}
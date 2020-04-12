using Newtonsoft.Json;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Default Admin object
    /// </summary>
    public class DefaultAdmin
    {
        /// <summary>
        /// Admin username
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Admin email
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Admin password
        /// </summary>
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}

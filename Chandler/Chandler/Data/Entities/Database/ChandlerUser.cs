using Microsoft.AspNetCore.Identity;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// ChandlerUser object
    /// </summary>
    public class ChandlerUser : IdentityUser
    {
        /// <summary>
        /// Information about the admin status of the user
        /// </summary>
        public AdminInfo AdminInfo { get; set; } = new AdminInfo();

        /// <summary>
        /// The Discord Snowflake Id of the user who signed in using discord
        /// </summary>
        public ulong? DiscordId { get; set; } = null;
    }
}

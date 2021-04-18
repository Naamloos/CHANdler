using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.EF.Entities.Main
{
    /// <summary>
    /// ChandlerUser object
    /// </summary>
    public class ChandlerUser : IdentityUser<Guid>
    {
        /// <summary>
        /// The Discord Snowflake Id of the user who signed in using discord
        /// </summary>
        public ulong? DiscordId { get; private set; }

        public Guid? AdminInfoId { get; private set; }

        /// <summary>
        /// Information about the admin status of the user
        /// </summary>
        public virtual AdminInfo AdminInfo { get; set; }
    }
}

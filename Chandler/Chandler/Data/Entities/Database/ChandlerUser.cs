using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// ChandlerUser object
    /// </summary>
    public class ChandlerUser : IdentityUser
    {
        /// <summary>
        /// Whether or not the user is an Admin of the entire server
        /// </summary>
        public bool IsServerAdmin { get; set; } = false;

        /// <summary>
        /// Whether or not the user is the Admin of a board
        /// </summary>
        public bool IsBoardAdmin { get; set; } = false;

        /// <summary>
        /// The board the user is an admin of, if any.
        /// </summary>
        public string AdminOf { get; set; } = null;
    }
}

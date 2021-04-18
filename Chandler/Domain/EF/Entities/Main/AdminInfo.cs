using Domain.EF.Entities.Base;
using System.Collections.Generic;

namespace Domain.EF.Entities.Main
{
    /// <summary>
    /// Admin object
    /// </summary>
    public class AdminInfo : Entity
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

        public virtual ICollection<ChandlerUser> Users { get; set; }
    }
}

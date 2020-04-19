using System.ComponentModel.DataAnnotations;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Admin object
    /// </summary>
    public class AdminInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public uint Id { get; set; }

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

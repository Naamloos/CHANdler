using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Board Object
    /// </summary>
    [Table("boards")]
    public class Board
    {
        /// <summary>
        /// The tag name for the board
        /// </summary>
        [Key]
        [Column("tag")]
        public string Tag { get; set; }

        /// <summary>
        /// The actual name for the board
        /// </summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>
        /// The description for the board
        /// </summary>
        [Column("description")]
        public string Description { get; set; }

        /// <summary>
        /// The image url for the board
        /// </summary>
        [Column("image")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The board's message of the day
        /// </summary>
        [Column("motd")]
        public string Motd { get; set; }
    }
}

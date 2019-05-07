using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    [Table("boards")]
    public class Board
    {
        [Key]
        [Column("tag")]
        public string Tag { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("image")]
        public string ImageUrl { get; set; }

        [Column("motd")]
        public string Motd { get; set; }
    }
}

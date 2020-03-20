using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    [Table("passwords")]
    public class Password
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("hash")]
        public string Hash { get; set; }

        [Column("salt")]
        public string Salt { get; set; }

        [Column("cycles")]
        public int Cycles { get; set; }
    }
}

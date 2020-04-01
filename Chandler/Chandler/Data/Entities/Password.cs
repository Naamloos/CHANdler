using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Password Table Class
    /// </summary>
    [Table("passwords")]
    public class Password
    {
        /// <summary>
        /// Id of the password (Public key)
        /// </summary>
        [Key]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Password Hash
        /// </summary>
        [Column("hash")]
        public string Hash { get; set; }

        /// <summary>
        /// Password Salt
        /// </summary>
        [Column("salt")]
        public string Salt { get; set; }
    }
}

using Domain.EF.Entities.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.EF.Entities.Main
{
    /// <summary>
    /// Password Table Class
    /// </summary>
    public class Password : Entity
    {
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

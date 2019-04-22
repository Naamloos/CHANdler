using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    [Table("bans")]
    public class Ban
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("ip")]
        public string Ip { get; set; }

        [Column("userid")]
        public ulong UserId { get; set; }

        [Column("reason")]
        public string Reason { get; set; }

        [Column("expire")]
        public string ExpireUnix { get; set; }
    }
}

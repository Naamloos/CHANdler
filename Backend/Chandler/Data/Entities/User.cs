using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    // TODO: make more secure
    [Table("users")]
    public class User
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("username")]
        public string Username { get; set; }

        [Column("pass")]
        public string Pass { get; set; }

        [Column("salt")]
        public string Salt { get; set; }

        [Column("ip")]
        public string Ip { get; set; }
    }
}

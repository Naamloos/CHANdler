using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    [Table("moderators")]
    public class Moderator
    {
        [Column("userid")]
        public ulong Id { get; set; }

        [Column("boardid")]
        public string ShortName { get; set; }
    }
}

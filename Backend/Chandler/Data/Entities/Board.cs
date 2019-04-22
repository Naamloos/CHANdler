using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    [Table("boards")]
    public class Board
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("shortname")]
        public string ShortName { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("banner")]
        public string BannerUrl { get; set; }

        [Column("motd")]
        public string Motd { get; set; }
    }
}

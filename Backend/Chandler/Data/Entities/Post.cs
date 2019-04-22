using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    [Table("posts")]
    public class Post
    {
        [Column("id")]
        public ulong Id { get; set; }

        [Column("date")]
        public ulong UnixDate { get; set; }

        [Column("parent")]
        public ulong ParentId { get; set; }

        [Column("image")]
        public string Image { get; set; }

        [Column("Username")]
        public string Username { get; set; }

        [Column("Userid")]
        public ulong UserId { get; set; }

        [Column("Topic")]
        public string Topic { get; set; }

        [Column("text")]
        public string Text { get; set; }

        [Column("boardid")]
        public ulong BoardId { get; set; }

        [Column("sticky")]
        public bool Sticky { get; set; }

        [Column("ip")]
        public string Ip { get; set; }
    }
}

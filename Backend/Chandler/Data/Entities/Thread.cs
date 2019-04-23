using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    [Table("thread")]
    public class Thread
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Column("parentid")]
        public int ParentId { get; set; } = -1;

        [Column("boardtag")]
        public string BoardTag { get; set; }

        [Column("image")]
        public string Image { get; set; } = "";

        [Column("username")]
        public string Username { get; set; } = "Anonymous";

        [Column("topic")]
        public string Topic { get; set; } = "";

        [Column("text")]
        public string Text { get; set; }

        [Column("ip")]
        public string Ip { get; set; }
    }
}

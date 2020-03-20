using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string Image { get; set; }

        [Column("username")]
        public string Username { get; set; } = "Anonymous";

        [Column("topic")]
        public string Topic { get; set; } = "";

        [Column("text")]
        public string Text { get; set; }

        [Column("ip")]
        public string Ip { get; set; }

        [Column("generatepass")]
        public string GeneratePassword { get; set; }

        [Column("passwordid")]
        public int PasswordId { get; set; }

        [Column("replytoid")]
        public long? ReplyToId { get; set; }

        [Column("iscommentreply")]
        public bool IsCommentReply { get => (this.ReplyToId > -1); }
    }
}

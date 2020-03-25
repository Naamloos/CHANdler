using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Thread Object
    /// </summary>
    [Table("thread")]
    public class Thread
    {
        /// <summary>
        /// ID of the thread
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// The ID to the parent of the thread (-1 if the thread has no parent)
        /// </summary>
        [Column("parentid")]
        public int ParentId { get; set; } = -1;

        /// <summary>
        /// The tag for the board this post belongs to
        /// </summary>
        [Column("boardtag")]
        public string BoardTag { get; set; }

        /// <summary>
        /// The image url for this thread
        /// </summary>
        [Column("image")]
        public string Image { get; set; }

        /// <summary>
        /// The name of the user who posted the thread ("Anonymous" by default)
        /// </summary>
        [Column("username")]
        public string Username { get; set; } = "Anonymous";

        /// <summary>
        /// The thread's topic
        /// </summary>
        [Column("topic")]
        public string Topic { get; set; } = "";

        /// <summary>
        /// The main message content for the thread
        /// </summary>
        [Column("text")]
        public string Text { get; set; }

        /// <summary>
        /// The IP Address of the poster
        /// </summary>
        [Column("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// The gernated password hash
        /// </summary>
        [Column("generatepass")]
        public string GeneratePassword { get; set; }

        /// <summary>
        /// The ID of the password in the database
        /// </summary>
        [Column("passwordid")]
        public int PasswordId { get; set; }

        /// <summary>
        /// The ID of the post to reply to. Required when mentioning another post using ">>ID".
        /// </summary>
        [Column("replytoid")]
        public long? ReplyToId { get; set; }

        /// <summary>
        /// Whether or not the comment is a reply
        /// </summary>
        [Column("iscommentreply")]
        public bool IsCommentReply { get => (this.ReplyToId > -1); }
    }
}

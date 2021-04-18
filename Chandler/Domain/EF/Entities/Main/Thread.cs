using Domain.EF.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.EF.Entities.Main
{
    /// <summary>
    /// Thread Object
    /// </summary>
    public class Thread : Entity
    {
        /// <summary>
        /// The ID to the parent of the thread (-1 if the thread has no parent)
        /// </summary>
        [Column("ParentId")]
        public Guid ParentId { get; set; } = Guid.Empty;

        /// <summary>
        /// The ID of the user who posted the thread ('-1' if the thread was posted anonymously)
        /// </summary>
        [Column("UserId")]
        public Guid UserId { get; set; } = Guid.Empty;

        /// <summary>
        /// The tag for the board this post belongs to
        /// </summary>
        [Column("BoardTag")]
        public string BoardTag { get; set; }

        /// <summary>
        /// The image url for this thread
        /// </summary>
        [Column("ImageUrl")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// The name of the user who posted the thread ("Anonymous" by default)
        /// </summary>
        [Column("Username")]
        public string Username { get; set; } = "Anonymous";

        /// <summary>
        /// The thread's topic
        /// </summary>
        [Column("Topic")]
        public string Topic { get; set; } = "";

        /// <summary>
        /// The main message content for the thread
        /// </summary>
        [Column("Text")]
        public string Text { get; set; }

        /// <summary>
        /// The IP Address of the poster
        /// </summary>
        [Column("Ip")]
        public string Ip { get; set; }

        /// <summary>
        /// The gernated password hash
        /// </summary>
        [Column("GeneratedPasswordHash")]
        public string GeneratedPasswordHash { get; set; }

        /// <summary>
        /// The ID of the password in the database
        /// </summary>
        [Column("PasswordId")]
        public Guid PasswordId { get; set; }

        /// <summary>
        /// The ID of the post to reply to. Required when mentioning another post using ">>ID".
        /// </summary>
        [Column("ReplyToId")]
        public Guid ReplyToId { get; set; }

        /// <summary>
        /// Whether or not the comment is a reply
        /// </summary>
        [Column("IsCommentReply")]
        public bool IsCommentReply { get => (this.ReplyToId != Guid.Empty); }

        public virtual Thread ParentThread { get; set; }

        /// <summary>
        /// Child threads for this thread
        /// </summary>
        public virtual IEnumerable<Thread> ChildThreads { get; set; }
    }
}

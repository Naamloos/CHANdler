namespace Chandler.Models
{
    /// <summary>
    /// New Post Page Model
    /// </summary>
    public class NewPostPageModel
    {
        /// <summary>
        /// Board to post to
        /// </summary>
        public string BoardTag { get; set; }

        /// <summary>
        /// Id of the thread this post belongs to
        /// </summary>
        public int ParentId { get; set; }
        
        /// <summary>
        /// Id of the post this post is replying to
        /// </summary>
        public long ReplyToId { get; set; }

        /// <summary>
        /// Whether or not the post a reply to a main thread comment
        /// </summary>
        public bool IsCommentReply { get => (this.ReplyToId > -1); }

        /// <summary>
        /// Is the post a reply to the main thread
        /// </summary>
        public bool IsThreadReply { get; set; }

        /// <summary>
        /// Configuration for the server
        /// </summary>
        public ServerConfig Config { get; set; }
    }
}

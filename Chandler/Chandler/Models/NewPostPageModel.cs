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
        /// Is the post a reply?
        /// </summary>
        public bool IsCommentReply { get => (this.ReplyToId > -1); }
    }
}

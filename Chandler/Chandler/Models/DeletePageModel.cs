namespace Chandler.Models
{
    /// <summary>
    /// Delete Page Model
    /// </summary>
    public class DeletePageModel
    {
        /// <summary>
        /// Id of the post to delte
        /// </summary>
        public int PostId { get; set; }

        /// <summary>
        /// Password for the post
        /// </summary>
        public string Password { get; set; }
        
        /// <summary>
        /// Board this post belongs to
        /// </summary>
        public string BoardTag { get; set; }
    }
}

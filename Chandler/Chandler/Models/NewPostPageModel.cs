namespace Chandler.Models
{
    public class NewPostPageModel
    {
        public string BoardTag { get; set; }
        public int ParentId { get; set; }
        public long ReplyToId { get; set; }
        public bool IsCommentReply { get => (this.ReplyToId > -1); }
    }
}

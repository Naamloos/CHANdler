namespace Chandler.Models
{
    public class DeletePageModel
    {
        public int PostId { get; set; }
        public string Password { get; set; }
        public string BoardTag { get; set; }
        public bool Failed { get; set; } = false;
    }
}

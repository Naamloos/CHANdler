using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    [Table("webhooksubscriptions")]
    public class WebhookSubscription
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("url")]
        public string Url { get; set; }

        [Column("boardtag")]
        public string BoardTag { get; set; }

        [Column("threadid")]
        public int? ThreadId { get; set; }

        [Column("hash")]
        public string Hash { get; set; }

        [Column("hashcycles")]
        public int HashCycles { get; set; }

        [Column("hashsalt")]
        public string HashSalt { get; set; }

        [Column("webhookUrlId")]
        public ulong UrlId { get; set; }
    }
}

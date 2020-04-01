using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Webhook Subscription Object
    /// </summary>
    [Table("webhooksubscriptions")]
    public class WebhookSubscription
    {
        /// <summary>
        /// ID of the webhook subscription
        /// </summary>
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// The token for the webhook
        /// </summary>
        [Column("token")]
        public string Token { get; set; }

        /// <summary>
        /// id for the webhook
        /// </summary>
        [Column("webhookid")]
        public ulong WebhookId { get; set; }

        /// <summary>
        /// The board tag to listen to
        /// </summary>
        [Column("boardtag")]
        public string BoardTag { get; set; }

        /// <summary>
        /// The ID of the thread to listen to
        /// </summary>
        [Column("threadid")]
        public int? ThreadId { get; set; }
    }
}

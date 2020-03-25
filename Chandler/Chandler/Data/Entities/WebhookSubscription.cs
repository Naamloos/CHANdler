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
        /// The url for the webhook
        /// </summary>
        [Column("url")]
        public string Url { get; set; }

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

        /// <summary>
        /// The password ID
        /// </summary>
        [Column("passwordid")]
        public int PasswordId { get; set; }

        /// <summary>
        /// ID of the webhook
        /// </summary>
        [Column("webhookUrlId")]
        public ulong UrlId { get; set; }
    }
}

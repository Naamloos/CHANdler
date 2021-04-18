using Domain.Misc;

namespace Chandler.Models
{
    /// <summary>
    /// This model gets used in the partial view for metadata
    /// </summary>
    public class MetadataViewModel
    {
        /// <summary>
        /// Page title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Page url (relative)
        /// </summary>
        public string RelativeUrl { get; set; }

        /// <summary>
        /// Image url
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// Whether the img url is relative
        /// </summary>
        public bool ImageRelative { get; set; }

        /// <summary>
        /// Page description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Configuration for the server
        /// </summary>
        public ServerConfig Config { get; set; }
    }
}

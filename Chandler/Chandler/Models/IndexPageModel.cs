using Chandler.Data.Entities;
using System.Collections.Generic;

namespace Chandler.Models
{
    /// <summary>
    /// Index Page Model
    /// </summary>
    public class IndexPageModel
    {
        /// <summary>
        /// List of avaliable board
        /// </summary>
        public IEnumerable<Board> Boards { get; set; }

        /// <summary>
        /// Configuration for the server
        /// </summary>
        public ServerConfig Config { get; set; }

        /// <summary>
        /// The status of an api action
        /// </summary>
        public ApiActionStatus ActionStatus { get; set; }
    }
}

using Chandler.Data.Entities;
using System.Collections.Generic;

namespace Chandler.Models
{
    /// <summary>
    /// Board Page Model
    /// </summary>
    public class BoardPageModel
    {
        /// <summary>
        /// Info for the board
        /// </summary>
        public Board BoardInfo { get; set; }

        /// <summary>
        /// Threads belonging to the board
        /// </summary>
        public IEnumerable<Thread> Threads { get; set; }

        /// <summary>
        /// Thread IDs for Threads with more than 5 responses
        /// </summary>
        public IEnumerable<int> BigThreads { get; set; }

        /// <summary>
        /// Amount of pages available
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Current page
        /// </summary>
        public int Currentpage { get; set; }

        /// <summary>
        /// Max threads per board page
        /// </summary>
        public int MaxThreadsPerPage { get; set; }

        /// <summary>
        /// Configuration for the server
        /// </summary>
        public ServerConfig Config { get; set; }

        /// <summary>
        /// Status of an Api action
        /// </summary>
        public ApiActionStatus ActionStatus { get; set; }
    }
}

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
    }
}

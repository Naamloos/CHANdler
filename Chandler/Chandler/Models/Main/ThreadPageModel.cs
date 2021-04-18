using Chandler.Data.Entities;
using Domain.EF.Entities.Main;
using Domain.Misc;

namespace Chandler.Models
{
    /// <summary>
    /// Board Page Model
    /// </summary>
    public class ThreadPageModel
    {
        /// <summary>
        /// Info for the board
        /// </summary>
        public Board BoardInfo { get; set; }

        /// <summary>
        /// Threads belonging to the board
        /// </summary>
        public Thread Thread { get; set; }

        /// <summary>
        /// Configuration for the server
        /// </summary>
        public ServerConfig Config { get; set; }
    }
}

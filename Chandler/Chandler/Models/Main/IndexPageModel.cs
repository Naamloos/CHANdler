using Domain.EF.Entities.Main;
using Domain.Misc;
using System.Linq;

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
        public IQueryable<Board> Boards { get; set; }

        /// <summary>
        /// Configuration for the server
        /// </summary>
        public ServerConfig Config { get; set; }
    }
}

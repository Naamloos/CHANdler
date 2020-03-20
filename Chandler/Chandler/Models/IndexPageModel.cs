using Chandler.Data.Entities;
using System.Collections.Generic;

namespace Chandler.Models
{
    public class IndexPageModel
    {
        public IEnumerable<Board> Boards { get; set; }

        public ServerConfig Config { get; set; }
    }
}

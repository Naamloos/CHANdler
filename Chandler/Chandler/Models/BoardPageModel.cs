using Chandler.Data.Entities;
using System.Collections.Generic;

namespace Chandler.Models
{
    public class BoardPageModel
    {
        public Board BoardInfo { get; set; }

        public IEnumerable<Thread> Threads { get; set; }
    }
}

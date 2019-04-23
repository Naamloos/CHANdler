using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data
{
    public class ServerMeta
    {
        public DateTime StartTime { get; }
        public TimeSpan UpTime => DateTime.Now.Subtract(StartTime);

        public ServerMeta()
        {
            this.StartTime = DateTime.Now;
        }
    }
}

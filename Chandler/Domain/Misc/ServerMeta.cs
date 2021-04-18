using System;

namespace Domain.Misc
{
    /// <summary>
    /// Server Meta
    /// </summary>
    public class ServerMeta
    {
        /// <summary>
        /// Time the server started
        /// </summary>
        public DateTime StartTime { get; }

        /// <summary>
        /// Total amount of time the server has been active for
        /// </summary>
        public TimeSpan UpTime => DateTime.Now.Subtract(StartTime);

        /// <summary>
        /// ServerMeta ctor
        /// </summary>
        public ServerMeta() => this.StartTime = DateTime.Now;
    }
}

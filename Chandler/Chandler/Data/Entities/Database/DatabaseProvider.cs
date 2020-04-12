using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data.Entities
{
    /// <summary>
    /// Database Provider Enum
    /// </summary>
    public enum DatabaseProvider : int
    {
        /// <summary>
        /// Use PostgreSQL
        /// </summary>
        PostgreSql = 0,

        /// <summary>
        /// Use SQLite
        /// </summary>
        Sqlite = 1,
        
        /// <summary>
        /// Use an in memory datebase
        /// </summary>
        InMemory = 2,

        /// <summary>
        /// Use an SqlServer
        /// </summary>
        SqlServer = 3
    }
}

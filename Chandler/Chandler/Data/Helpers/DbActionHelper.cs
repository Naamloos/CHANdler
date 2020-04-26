using System.Threading.Tasks;

namespace Chandler.Data
{
    /// <summary>
    /// Helper for doing actions on database from the Attribute
    /// </summary>
    public class DbActionHelper
    {
        /// <summary>
        /// Database
        /// </summary>
        public static Database Db { get; set; }

        /// <summary>
        /// DbActionHelper ctor
        /// </summary>
        /// <param name="db"></param>
        public DbActionHelper(Database db) => Db = db;

        /// <summary>
        /// Lock the db semaphore
        /// </summary>
        /// <returns></returns>
        public static async Task WaitLockSemaphoreAsync() => await Db.DbSema.WaitAsync().ConfigureAwait(false);
        
        /// <summary>
        /// Release the semaphore
        /// </summary>
        public static void ReleaseSemaphore() => Db.DbSema.Release();
    }
}

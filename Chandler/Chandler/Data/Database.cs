using System;
using Chandler.Data.Entities;

namespace Chandler.Data
{
    /// <summary>
    /// Database Object
    /// </summary>
    public class Database
    {
        private string ConnectionString { get; }
        private DatabaseProvider Provider { get; }

        /// <summary>
        /// Database Ctor
        /// </summary>
        /// <param name="provider">Database Provider</param>
        /// <param name="cstr">Database Connection String</param>
        public Database(DatabaseProvider provider, string cstr)
        {
            this.Provider = provider;
            this.ConnectionString = cstr;
        }

        /// <summary>
        /// Get the current database context
        /// </summary>
        /// <returns></returns>
        public DatabaseContext GetContext()
        {
            try
            {
                return new DatabaseContext(this.Provider, this.ConnectionString);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error during database initialization:");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

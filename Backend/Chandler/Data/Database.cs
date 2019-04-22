using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data
{
    public class Database
    {
        private string ConnectionString { get; }
        private DatabaseProvider Provider { get; }

        public Database(DatabaseProvider provider, string cstr)
        {
            this.Provider = provider;
            this.ConnectionString = cstr;
        }

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

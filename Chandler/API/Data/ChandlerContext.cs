using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data
{
    public class ChandlerContext : DbContext
    {
        private string _connectionString;

        public ChandlerContext(string connectionString)
            => _connectionString = connectionString;

        protected override void OnModelCreating(ModelBuilder builder) => builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
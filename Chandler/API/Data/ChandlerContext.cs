using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data
{
    public class ChandlerContext : DbContext
    {
        public ChandlerContext() { }

        public ChandlerContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) => builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
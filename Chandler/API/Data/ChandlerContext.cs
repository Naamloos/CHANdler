using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data
{
    public class ChandlerContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=my_host;Database=my_db;Username=my_user;Password=my_pw");

        protected override void OnModelCreating(ModelBuilder builder) => builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
using Chandler.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Chandler.Data
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<Thread> Threads { get; set; }
        public virtual DbSet<Password> Passwords { get; set; }

        private string ConnectionString { get; }
        public DatabaseProvider Provider { get; }

        public DatabaseContext(DatabaseProvider provider, string cstring)
        {
            this.Provider = provider;
            this.ConnectionString = cstring;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            optionsBuilder.EnableSensitiveDataLogging(true);

            switch (this.Provider)
            {
                case DatabaseProvider.PostgreSql:
                    optionsBuilder.UseNpgsql(this.ConnectionString);
                    break;

                case DatabaseProvider.Sqlite:
                    optionsBuilder.UseSqlite(this.ConnectionString);
                    break;

                case DatabaseProvider.ServiceProvider:
                    optionsBuilder.UseInternalServiceProvider(new ServiceCollection()
                        .AddSingleton(Boards)
                        .AddSingleton(Threads)
                        .AddSingleton(Passwords)
                        .BuildServiceProvider());
                    break;

                ///EF Core 3.0 is gay
                /*case DatabaseProvider.InMemory:
                    optionsBuilder.UseInMemoryDatabase("chandler");
                    break;*/
            }
        }
    }

    public enum DatabaseProvider
    {
        PostgreSql,
        Sqlite,
        InMemory,
        ServiceProvider
    }
}

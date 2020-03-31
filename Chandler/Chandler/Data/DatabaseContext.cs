using Chandler.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chandler.Data
{
    /// <summary>
    /// Database Context Object
    /// </summary>
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// List of boards
        /// </summary>
        public DbSet<Board> Boards { get; set; }

        /// <summary>
        /// List of threads
        /// </summary>
        public DbSet<Thread> Threads { get; set; }

        /// <summary>
        /// List of passwords
        /// </summary>
        public DbSet<Password> Passwords { get; set; }

        /// <summary>
        /// List of webhook subscriptions
        /// </summary>
        public DbSet<WebhookSubscription> WebhookSubscritptions { get; set; }

        private string ConnectionString { get; }

        private DatabaseProvider Provider { get; }

        /// <summary>
        /// DatabaseContext Ctor
        /// </summary>
        /// <param name="provider">Database Provider</param>
        /// <param name="cstring">Database Connection String</param>
        public DatabaseContext(DatabaseProvider provider, string cstring)
        {
            this.Provider = provider;
            this.ConnectionString = cstring;
        }

        /// <summary>
        /// Overridden version of OnConfiguring
        /// </summary>
        /// <param name="optionsBuilder">DbContextOptionsBuilder</param>
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

                case DatabaseProvider.InMemory:
                    optionsBuilder.UseInMemoryDatabase("chandler");
                    break;
            }
        }
    }
}

using Chandler.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;

namespace Chandler.Data
{
    /// <summary>
    /// Database Object
    /// </summary>
    public class Database : IdentityDbContext<ChandlerUser>
    {
        /// <summary>
        /// Semaphore for the database
        /// </summary>
        public SemaphoreSlim DbSema { get; set; }

        /// <summary>
        /// List of boards
        /// </summary>
        public DbSet<Board> Boards { get; set; }

        /// <summary>
        /// List of threads
        /// </summary>
        public DbSet<Entities.Thread> Threads { get; set; }

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
        public Database(DatabaseProvider provider, string cstring)
        {
            this.Provider = provider;
            this.ConnectionString = cstring;
            this.DbSema = new SemaphoreSlim(1, 1);
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
                case DatabaseProvider.InMemory:
                    optionsBuilder.UseInMemoryDatabase("chandler");
                    break;

                case DatabaseProvider.PostgreSql:
                    optionsBuilder.UseNpgsql(this.ConnectionString);
                    break;

                case DatabaseProvider.Sqlite:
                    optionsBuilder.UseSqlite(this.ConnectionString);
                    break;

                case DatabaseProvider.SqlServer:
                    optionsBuilder.UseSqlServer(this.ConnectionString);
                    break;

                default:
                    throw new ArgumentException("DatabaseProvider is an invalid value. Valid values are: 0, 1, 2, and 3");
            }
        }
    }
}
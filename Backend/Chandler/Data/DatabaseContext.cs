using Chandler.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chandler.Data
{
    public class DatabaseContext : DbContext
    {
        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<Thread> Threads { get; set; }

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
                // TODO: postgres and sqlite support.
                case DatabaseProvider.PostgreSql:
                    //optionsBuilder.UseNpgsql(this.ConnectionString);

                case DatabaseProvider.Sqlite:
                    //optionsBuilder.UseSqlite(this.ConnectionString);

                case DatabaseProvider.InMemory:
                    optionsBuilder.UseInMemoryDatabase("chandler");
                    break;
            }
        }
    }

    public enum DatabaseProvider
    {
        PostgreSql,
        Sqlite,
        InMemory
    }
}

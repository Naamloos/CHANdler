using Chandler.Data.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using System;
using System.Collections.Generic;

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

                //Broken: SqliteException: SQLite Error 19: 'UNIQUE constraint failed: boards.tag'.
                case DatabaseProvider.Sqlite:
                    optionsBuilder.UseSqlite(this.ConnectionString);
                    break;

                case DatabaseProvider.InMemory:
                    optionsBuilder.UseInMemoryDatabase("chandler");
                    break;
            }
        }
    }

    public enum DatabaseProvider : int
    {
        PostgreSql = 0,
        Sqlite = 1,
        InMemory = 2
    }
}

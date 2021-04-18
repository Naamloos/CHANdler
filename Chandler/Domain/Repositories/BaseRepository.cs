using Domain.EF.Entities;
using Domain.EF.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public abstract class BaseRepository<TEntity> where TEntity : Entity
    {
        private ChandlerContext Context { get; }
        private DbSet<TEntity> DbSet { get; }

        public BaseRepository(ChandlerContext ctx)
        {
            this.Context = ctx;
            this.DbSet = this.Context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAll() => this.DbSet;

        public async Task<TEntity> GetByIdAsync(Guid id) => await this.DbSet.FindAsync(id);

        public async Task InsertAsync(TEntity entity)
        {
            await this.DbSet.AddAsync(entity);
            await this.Context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            this.DbSet.Update(entity);
            await this.Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            this.DbSet.Remove(entity);
            await this.Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await this.DbSet.FindAsync(id);
            if (entity != null) await this.DeleteAsync(entity);
        } 
    }
}

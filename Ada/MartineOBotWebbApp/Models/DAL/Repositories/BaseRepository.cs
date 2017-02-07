using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace MartineOBotWebApp.Models.DAL.Repositories
{
    public class BaseRepository<TEntity> where TEntity : class
    {
        public ApplicationDbContext Context { get; }
        public DbSet<TEntity> Table { get; }

        public BaseRepository(ApplicationDbContext context)
        {
            Context = context;
            Table = Context.Set<TEntity>(); 
        }

        public async Task<IEnumerable<TEntity>> GetAll()
        {
            return await Table.ToListAsync(); 
        }

        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await Table.FindAsync(id);
        }

        public void Insert(TEntity newEntity)
        {
            Table.Add(newEntity); 
        }

        public void Update(TEntity entityToUpdate)
        {
            Table.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified; 
        }

        public async Task Remove(object idEntityToDelete)
        {
            var entity = await GetByIdAsync(idEntityToDelete);
            if (entity != null) Remove(entity); 
        }

        public void Remove(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
                Table.Attach(entityToDelete);

            Table.Remove(entityToDelete); 
        }

        public void RemoveAll()
        {
            Table.RemoveRange(Table); 
        }
    }
}
using LibraryManagement.Data;
using LibraryManagement.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryManagement.Models.Repository.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private LibraryContext _db;
        internal DbSet<T> dbSet;

        public Repository(LibraryContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> propertyName)
        {
            return await dbSet.FirstOrDefaultAsync(propertyName);
        }
        public async Task<T> GetSingleAsync (Expression<Func<T, bool>> predicate, string includeProperties = "")
        {
            IQueryable<T> query = dbSet;

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(Expression<Func<T, bool>> propertyName)
        {
            T entity = await dbSet.FirstOrDefaultAsync(propertyName);
            if(entity != null)
            {
                dbSet.Remove(entity);
                await SaveAsync();
            }
        }
        public async Task<bool> CheckDuplicateAtCreation(Expression<Func<T, bool>> propertyName)
        {
            return await dbSet.AnyAsync(propertyName);
        }
        public async Task UpdateAsync(T entity)
        {
            _db.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
        }
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

        public Task RemoveObjAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}

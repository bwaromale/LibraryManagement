using System.Linq.Expressions;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T, bool>> propertyName);
        Task CreateAsync(T entity);
        Task RemoveAsync(Expression<Func<T, bool>> propertyName);
        Task<bool> CheckDuplicateAtCreation(Expression<Func<T,bool>> propertyName);
        Task UpdateAsync(T entity);
        Task SaveAsync();
    }
}

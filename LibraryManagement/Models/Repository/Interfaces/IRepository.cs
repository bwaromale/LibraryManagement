using System.Linq.Expressions;

namespace LibraryManagement.Models.Repository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task CreateAsync(T entity);
        Task<bool> CheckDuplicateAtCreation(Expression<Func<T,bool>> propertyName);
        Task SaveAsync();
    }
}

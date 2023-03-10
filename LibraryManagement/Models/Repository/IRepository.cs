using System.Linq.Expressions;

namespace LibraryManagement.Models.Repository
{
    public interface IRepository<T> where T : class
    { 
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task CreateAsync(T entity);
        Task SaveAsync();
    }
}

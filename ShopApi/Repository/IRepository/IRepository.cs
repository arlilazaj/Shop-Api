using System.Linq.Expressions;

namespace ShopApi.Repository.IRepository;

public interface IRepository<T> where T : class
{
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,string? includeProperties=null,int pageSize=0,int pageNumber=1,Expression<Func<T, object>>? orderBy = null, bool orderByDescending = false);
    Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked=true,string? includeProperties=null);
    Task CreateAsync(T entity);
    Task DeleteAsync(T entity);
    Task SaveAsync();
}
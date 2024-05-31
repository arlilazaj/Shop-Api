using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using ShopApi.Data;
using ShopApi.Repository.IRepository;
using System.Linq;
namespace ShopApi.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private ApplicationDbContext _db;
    private DbSet<T> dbset;
    public Repository(ApplicationDbContext db)
    {
        _db = db;
        this.dbset = _db.Set<T>();
    }

   public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,string?  includeProperties=null,int pageSize=0,int pageNumber=1, Expression<Func<T, object>>? orderBy = null, bool orderByDescending = false)
    {
        IQueryable<T> query = dbset;
        if (filter!=null)
        {
            query = query.Where(filter);
        }

        if (orderBy!=null)
        {
            query = orderByDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        if (pageSize>0)
        {
            if (pageSize>100)
            {
                pageSize = 100;
            }

            query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
        }
        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        
        return await query.ToListAsync();
    }
    
 public async  Task<T> GetAsync(Expression<Func<T, bool>>? filter = null, bool tracked = true,string? includeProperties=null)
   {
       IQueryable<T> query = dbset;
       if (!tracked)
       {
           query = query.AsNoTracking();
       }

       if (filter!=null)
       {
           query=query.Where(filter);
       }
       if (includeProperties != null)
       {
           foreach (var includeProp in includeProperties.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries) )
           {
               query = query.Include(includeProp);  
           }
       }
       return await query.FirstOrDefaultAsync();
   }

public async Task CreateAsync(T entity)
{
  await  dbset.AddAsync(entity);
  await  SaveAsync();
}
public async Task DeleteAsync(T entity)
{
    dbset.Remove(entity);
    await SaveAsync();
}

public async Task SaveAsync()
{
    await  _db.SaveChangesAsync();
}
}
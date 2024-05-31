using ShopApi.Data;
using ShopApi.Model;
using ShopApi.Repository.IRepository;

namespace ShopApi.Repository;

public class CategoryRepository :  Repository<Category>, ICategoryRepository
{
    private readonly ApplicationDbContext _db;
    public CategoryRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    
    public async Task<Category> UpdateAsync(Category entity)
    {
        entity.UpdatedDate = DateTime.Now;
        _db.Categories.Update(entity);
       await  SaveAsync();
       return entity;
    }
}
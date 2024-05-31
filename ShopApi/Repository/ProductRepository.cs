using ShopApi.Data;
using ShopApi.Model;
using ShopApi.Repository.IRepository;

namespace ShopApi.Repository;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _db;
    public ProductRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public async Task<Product> UpdateAsync(Product entity)
    {
        entity.UpdatedDate = DateTime.Now;
       _db.Products.Update(entity);
       await SaveAsync();
       return entity;
    }
}
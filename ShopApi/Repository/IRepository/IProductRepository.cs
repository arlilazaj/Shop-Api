using ShopApi.Model;

namespace ShopApi.Repository.IRepository;

public interface IProductRepository:IRepository<Product>

{
    Task<Product> UpdateAsync(Product entity);    
}
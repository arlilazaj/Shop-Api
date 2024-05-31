using ShopApi.Model;

namespace ShopApi.Repository.IRepository;

public interface IOrderRepository:IRepository<Model.Order>
{
    Task<Model.Order> UpdateAsync(Model.Order entity);   
}
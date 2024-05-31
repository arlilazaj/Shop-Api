using Microsoft.EntityFrameworkCore.TestModels.Northwind;
using ShopApi.Data;
using ShopApi.Repository.IRepository;

namespace ShopApi.Repository;

public class OrderRepository:Repository<Model.Order>,IOrderRepository
{
    private readonly ApplicationDbContext _db;
    public OrderRepository(ApplicationDbContext dbContext):base(dbContext)
    {
        _db = dbContext;
    }

    public async Task<Model.Order> UpdateAsync(Model.Order entity)
    {
        entity.UpdatedAt=DateTime.Now;
        _db.Orders.Update(entity);
        await SaveAsync();
        return entity;
    }
}
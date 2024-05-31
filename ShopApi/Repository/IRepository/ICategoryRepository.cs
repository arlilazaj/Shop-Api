using Microsoft.EntityFrameworkCore.TestModels.UpdatesModel;
using Category = ShopApi.Model.Category;

namespace ShopApi.Repository.IRepository;

public interface ICategoryRepository:IRepository<Category>
{
    Task<Model.Category> UpdateAsync(Model.Category entity);
}
using Domain.Entities;

namespace Application.Abstractions;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<IEnumerable<Category>> GetSubCategoriesAsync(long parentId);
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
    Task<IEnumerable<Category>> GetFeaturedCategoriesAsync();
    Task<int> GetProductCountAsync(long categoryId);
    Task<bool> HasChildrenAsync(long categoryId);
}

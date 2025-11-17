using WebApplication3.Models;
using WebApplication3.Repositories;

namespace WebApplication3.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task<IEnumerable<Category>> SearchCategoriesAsync(string query);
        Task CreateCategoryAsync(Category category);
        Task EditCategoryAsync(Category category);
        Task RemoveCategoryAsync(int id);
    }

    
}

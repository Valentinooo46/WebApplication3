using WebApplication3.Models;
using WebApplication3.Repositories;

namespace WebApplication3.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Category>> GetCategoriesAsync() => _repository.GetAllAsync();
        public Task<Category> GetCategoryAsync(int id) => _repository.GetByIdAsync(id);
        public Task<IEnumerable<Category>> SearchCategoriesAsync(string query) => _repository.SearchAsync(query);
        public Task CreateCategoryAsync(Category category) => _repository.AddAsync(category);
        public Task EditCategoryAsync(Category category) => _repository.UpdateAsync(category);
        public Task RemoveCategoryAsync(int id) => _repository.DeleteAsync(id);
    }
}

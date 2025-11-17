using WebApplication3.Models;

namespace WebApplication3.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly HttpClient _httpClient;

        public CategoryRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://lohika.itstep.click/api/Categories/");
        }

        public async Task<IEnumerable<Category>> GetAllAsync() //ОК
        {
            var response = await _httpClient.GetAsync("list");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
        }

        public async Task<Category> GetByIdAsync(int id) //ОК
        {
            var response = await _httpClient.GetAsync($"get/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Category>();
        }

        public async Task<IEnumerable<Category>> SearchAsync(string query)
        {
            var response = await _httpClient.GetAsync($"search?query={query}");   //потребує зміни
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<Category>>();
        }

        public async Task AddAsync(Category category) //ОК
        {
            var response = await _httpClient.PostAsJsonAsync("add", category);
            var result = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(Category category) //?
        {
            var response = await _httpClient.PutAsJsonAsync("edit", category);
            await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id) //ОК
        {
            var response = await _httpClient.DeleteAsync($"delete/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}

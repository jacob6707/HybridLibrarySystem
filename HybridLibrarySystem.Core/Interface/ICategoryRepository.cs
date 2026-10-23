using HybridLibrarySystem.Core.Model;

namespace HybridLibrarySystem.Core.Interface;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> GetCategoryWithBooksByIdAsync(int id);
    Task<Category> AddCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
    Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
}
using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace HybridLibrarySystem.Data.Repository;

internal sealed class CategoryRepository(LibraryContext db) : ICategoryRepository
{
    public async Task<Category> AddCategoryAsync(Category category) {
        await db.Categories.AddAsync(category);
        await db.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await db.Categories.FindAsync(id);
        if (category == null) return false;
        db.Categories.Remove(category);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await db.Categories.AsNoTracking().ToListAsync();
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        var category = await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return category ?? throw new KeyNotFoundException($"Category with ID {id} not found.");
    }

    public async Task<Category> GetCategoryWithBooksByIdAsync(int id)
    {
        var category = await db.Categories
            .AsNoTracking()
            .Include(c => c.Books)
            .FirstOrDefaultAsync(c => c.Id == id);
        return category ?? throw new KeyNotFoundException($"Category with ID {id} not found.");
    }

    public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await db.Categories.AsNoTracking().ToListAsync();

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => c.Name != null && EF.Functions.Like(c.Name, $"%{searchTerm}%"))
            .ToListAsync();

        return categories;
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        var existingCategory = await db.Categories.FindAsync(category.Id) ?? throw new KeyNotFoundException($"Category with ID {category.Id} not found.");
        existingCategory.Name = category.Name;
        db.Categories.Update(existingCategory);
        await db.SaveChangesAsync();
        return existingCategory;
    }
}
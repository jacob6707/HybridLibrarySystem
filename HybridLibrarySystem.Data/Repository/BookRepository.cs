using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace HybridLibrarySystem.Data.Repository;

internal sealed class BookRepository(LibraryContext db) : IBookRepository
{
    public async Task<Book> AddBookAsync(Book book)
    {
        db.Books.Add(book);
        await db.SaveChangesAsync();
        return book;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await db.Books.FindAsync(id);
        if (book == null)
        {
            return false;
        }

        db.Books.Remove(book);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await db.Books.Include(b => b.Author).Include(b => b.Category).ToListAsync();
    }

    public async Task<Book> GetBookByIdAsync(int id)
    {
        return await db.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.Id == id)
               ?? throw new KeyNotFoundException($"Book with ID {id} not found.");
    }

    public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId)
    {
        return await db.Books.Include(b => b.Author).Include(b => b.Category).Where(b => b.AuthorId == authorId).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetBooksByCategoryIdAsync(int categoryId)
    {
        return await db.Books.Include(b => b.Author).Include(b => b.Category).Where(b => b.CategoryId == categoryId).ToListAsync();
    }

    public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
    {
        return await db.Books.Include(b => b.Author).Include(b => b.Category)
            .Where(b => b.Title!.Contains(searchTerm) || b.Author!.Name!.Contains(searchTerm) || b.Category!.Name!.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<Book> UpdateBookAsync(Book book)
    {
        db.Books.Update(book);
        await db.SaveChangesAsync();
        return book;
    }
}
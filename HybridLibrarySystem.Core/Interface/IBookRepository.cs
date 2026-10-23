using HybridLibrarySystem.Core.Model;

namespace HybridLibrarySystem.Core.Interface;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<Book> GetBookByIdAsync(int id);
    Task<Book> AddBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
    Task<bool> DeleteBookAsync(int id);
    Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
    Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(int authorId);
    Task<IEnumerable<Book>> GetBooksByCategoryIdAsync(int categoryId);
}
using HybridLibrarySystem.Core.Model;
using HybridLibrarySystem.Core.Model.DTO;

namespace HybridLibrarySystem.Core.Interface;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAuthorsAsync();
    Task<IEnumerable<AuthorSummary>> GetAuthorSummariesAsync();
    Task<Author> GetAuthorByIdAsync(int id);
    Task<Author> GetAuthorWithBooksByIdAsync(int id);
    Task<Author> AddAuthorAsync(Author author);
    Task<Author> UpdateAuthorAsync(Author author);
    Task<bool> DeleteAuthorAsync(int id);
    Task<IEnumerable<Author>> SearchAuthorsByNameAsync(string searchTerm);
    Task<IEnumerable<AuthorSummary>> SearchAuthorSummariesByNameAsync(string searchTerm);
}
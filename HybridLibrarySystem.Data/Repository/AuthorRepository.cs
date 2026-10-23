using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using HybridLibrarySystem.Core.Model.DTO;
using Microsoft.EntityFrameworkCore;

namespace HybridLibrarySystem.Data.Repository;

internal sealed class AuthorRepository(LibraryContext db) : IAuthorRepository
{
    public async Task<Author> AddAuthorAsync(Author author)
    {
        db.Authors.Add(author);
        await db.SaveChangesAsync();
        return author;
    }

    public async Task<bool> DeleteAuthorAsync(int id)
    {
        var author = db.Authors.Find(id);
        if (author == null)
        {
            return false;
        }
        db.Authors.Remove(author);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
    {
        return await db.Authors.AsNoTracking().ToListAsync();
    }

    public async Task<Author> GetAuthorByIdAsync(int id)
    {
        return await db.Authors.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id)
               ?? throw new KeyNotFoundException($"Author with ID {id} not found.");
    }

    public async Task<IEnumerable<AuthorSummary>> GetAuthorSummariesAsync()
    {
        return await db.Authors.AsNoTracking()
            .Select(a => new AuthorSummary
            (
                a.Id,
                a.Name,
                a.Biography,
                a.DateOfBirth,
                a.DateOfDeath,
                a.Nationality,
                a.Books!.Count
            ))
            .ToListAsync();
    }

    public async Task<Author> GetAuthorWithBooksByIdAsync(int id)
    {
        return await db.Authors.AsNoTracking()
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id)
            ?? throw new KeyNotFoundException($"Author with ID {id} not found.");
    }

    public async Task<IEnumerable<Author>> SearchAuthorsByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAuthorsAsync();
        }
        return await db.Authors.AsNoTracking()
            .Where(a => a.Name!.Contains(searchTerm))
            .ToListAsync();
    }

    public async Task<IEnumerable<AuthorSummary>> SearchAuthorSummariesByNameAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAuthorSummariesAsync();
        }
        return await db.Authors.AsNoTracking()
            .Where(a => a.Name!.Contains(searchTerm))
            .Select(a => new AuthorSummary
            (
                a.Id,
                a.Name,
                a.Biography,
                a.DateOfBirth,
                a.DateOfDeath,
                a.Nationality,
                a.Books!.Count
            ))
            .ToListAsync();
    }

    public async Task<Author> UpdateAuthorAsync(Author author)
    {
        db.Authors.Update(author);
        await db.SaveChangesAsync();
        return author;
    }
}
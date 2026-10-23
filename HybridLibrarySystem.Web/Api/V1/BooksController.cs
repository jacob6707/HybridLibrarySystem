using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Api.V1;
public class BooksController(IBookRepository dbBooks) : V1ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        // convert to DTOs
        var books = await dbBooks.GetAllBooksAsync();
        var dtoBooks = books.Select(b => new BookDTO(
            b.Id,
            b.Title,
            b.ISBN,
            new AuthorDTO(b.Author.Id, b.Author.Name, b.Author.Biography, b.Author.DateOfBirth, b.Author.DateOfDeath, b.Author.Nationality),
            new CategoryDTO(b.Category.Id, b.Category.Name),
            b.PublishedDate,
            b.CopiesAvailable
        )).ToList();
        return Ok(dtoBooks);
    }

    public record PostBookReuqest(string Title);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PostBook([FromBody] PostBookReuqest request)
    {
        return Ok(request);
    }
}

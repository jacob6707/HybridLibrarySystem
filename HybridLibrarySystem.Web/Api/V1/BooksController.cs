using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Core.Model.DTO;
using HybridLibrarySystem.Web.Features.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace HybridLibrarySystem.Web.Api.V1;

public class BooksController(
    IBookRepository dbBooks,
    IAuthorRepository dbAuthors,
    ICategoryRepository dbCategories,
    IReservationRepository dbReservations) : V1ApiControllerBase
{
    public record UpdateBookRequest(
        [Required, MaxLength(200)] string Title,
        [Required, MaxLength(13)] string ISBN,
        int AuthorId,
        int CategoryId,
        DateTime PublishedDate,
        [Range(0, int.MaxValue)] int CopiesAvailable
    );

    private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

    private static BookDTO ToDto(Book b, bool isReservedByMe) => new(
        b.Id,
        b.Title,
        b.ISBN,
        new AuthorDTO(b.Author!.Id, b.Author.Name, b.Author.Biography, b.Author.DateOfBirth, b.Author.DateOfDeath, b.Author.Nationality),
        new CategoryDTO(b.Category!.Id, b.Category.Name),
        b.PublishedDate,
        b.CopiesAvailable,
        isReservedByMe
    );

    private async Task<HashSet<int>> GetMyReservedBookIdsAsync()
    {
        var userId = CurrentUserId;
        return userId is null ? [] : await dbReservations.GetReservedBookIdsAsync(userId);
    }

    private async Task<Book?> FindBookAsync(int id)
    {
        try
        {
            return await dbBooks.GetBookByIdAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetBooks()
    {
        var books = await dbBooks.GetAllBooksAsync();
        var reservedIds = await GetMyReservedBookIdsAsync();
        return Ok(books.Select(b => ToDto(b, reservedIds.Contains(b.Id))).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBook(int id)
    {
        var book = await FindBookAsync(id);
        if (book is null)
        {
            return NotFound();
        }
        var reservedIds = await GetMyReservedBookIdsAsync();
        return Ok(ToDto(book, reservedIds.Contains(book.Id)));
    }

    [HttpPut("{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.UpdateBook)]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
    {
        var book = await FindBookAsync(id);
        if (book is null)
        {
            return NotFound();
        }

        try
        {
            await dbAuthors.GetAuthorByIdAsync(request.AuthorId);
        }
        catch (KeyNotFoundException)
        {
            ModelState.AddModelError(nameof(request.AuthorId), "Author does not exist.");
        }
        try
        {
            await dbCategories.GetCategoryByIdAsync(request.CategoryId);
        }
        catch (KeyNotFoundException)
        {
            ModelState.AddModelError(nameof(request.CategoryId), "Category does not exist.");
        }
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        book.Title = request.Title;
        book.ISBN = request.ISBN;
        book.AuthorId = request.AuthorId;
        book.CategoryId = request.CategoryId;
        book.PublishedDate = request.PublishedDate;
        book.CopiesAvailable = request.CopiesAvailable;
        await dbBooks.UpdateBookAsync(book);

        var updated = await dbBooks.GetBookByIdAsync(id);
        var reservedIds = await GetMyReservedBookIdsAsync();
        return Ok(ToDto(updated, reservedIds.Contains(id)));
    }

    [HttpDelete("{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.DeleteBook)]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (await FindBookAsync(id) is null)
        {
            return NotFound();
        }
        if (await dbReservations.BookHasReservationsAsync(id))
        {
            return Conflict(new ProblemDetails
            {
                Title = "Book has active reservations",
                Detail = "A book cannot be deleted while it has active reservations."
            });
        }

        try
        {
            var deleted = await dbBooks.DeleteBookAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (DbUpdateException)
        {
            // A reservation was created between the check and the delete; the FK restriction rejected it.
            return Conflict(new ProblemDetails
            {
                Title = "Book has active reservations",
                Detail = "A book cannot be deleted while it has active reservations."
            });
        }
    }

    [HttpPost("{id:int}/reservation")]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.ReserveBook)]
    public async Task<IActionResult> Reserve(int id)
    {
        var result = await dbReservations.ReserveAsync(id, CurrentUserId!);
        return result switch
        {
            ReserveResult.Reserved => NoContent(),
            ReserveResult.BookNotFound => NotFound(),
            ReserveResult.AlreadyReserved => Conflict(new ProblemDetails
            {
                Title = "Already reserved",
                Detail = "You have already reserved this book."
            }),
            ReserveResult.NoCopiesAvailable => Conflict(new ProblemDetails
            {
                Title = "No copies available",
                Detail = "There are no copies of this book available to reserve."
            }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}/reservation")]
    [ValidateAntiForgeryToken]
    [RequirePermission(Permissions.ReserveBook)]
    public async Task<IActionResult> Unreserve(int id)
    {
        // Lookup is scoped to the caller, so someone else's reservation is indistinguishable from none.
        var removed = await dbReservations.UnreserveAsync(id, CurrentUserId!);
        return removed ? NoContent() : NotFound();
    }
}

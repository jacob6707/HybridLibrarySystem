using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Web.Features.Authorization;
using HybridLibrarySystem.Web.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HybridLibrarySystem.Web.Controllers.Admin
{
    [Area("Admin")]
    public class BooksController(IBookRepository dbBooks, IAuthorRepository dbAuthors, ICategoryRepository dbCategories, IReservationRepository dbReservations) : Controller
    {
        // GET: BooksController
        [RequirePermission(Permissions.ReadBook,Permissions.CreateBook)]
        public async Task<IActionResult> Index()
        {
            var books = await dbBooks.GetAllBooksAsync();
            return View(books);
        }

        // GET: BooksController/Details/5
        [RequirePermission(Permissions.ReadBook,Permissions.CreateBook)]
        public async Task<IActionResult> Details(int id)
        {
            var book = await dbBooks.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // GET: BooksController/Create
        [RequirePermission(Permissions.CreateBook)]
        public async Task<IActionResult> Create()
        {
            var authors = await dbAuthors.GetAllAuthorsAsync();
            var categories = await dbCategories.GetAllCategoriesAsync();
            BookFormViewModel viewModel = new()
            {
                Book = null,
                Authors = new SelectList(authors, "Id", "Name"),
                Categories = new SelectList(categories, "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: BooksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.CreateBook)]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                var newBook = new Book
                {
                    Title = collection["Book.Title"],
                    AuthorId = int.Parse(collection["Book.AuthorId"]!),
                    CategoryId = int.Parse(collection["Book.CategoryId"]!),
                    PublishedDate = DateTime.Parse(collection["Book.PublishedDate"]!),
                    ISBN = collection["Book.ISBN"],
                    CopiesAvailable = int.Parse(collection["Book.CopiesAvailable"]!)
                };
                var createdBook = await dbBooks.AddBookAsync(newBook);
                TempData["Message"] = createdBook is not null ? $"Book with ID {createdBook.Id} created successfully!" : "Failed to create book.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while creating the book.";
                return View();
            }
        }

        // GET: BooksController/Edit/5
        [RequirePermission(Permissions.UpdateBook)]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await dbBooks.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var authors = await dbAuthors.GetAllAuthorsAsync();
            var categories = await dbCategories.GetAllCategoriesAsync();
            BookFormViewModel viewModel = new BookFormViewModel
            {
                Book = book,
                Authors = new SelectList(authors, "Id", "Name"),
                Categories = new SelectList(categories, "Id", "Name")
            };
            return View(viewModel);
        }

        // POST: BooksController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.UpdateBook)]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                var existingBook = await dbBooks.GetBookByIdAsync(id);
                if (existingBook is null)
                {
                    return NotFound();
                }
                existingBook.Title = collection["Book.Title"];
                existingBook.AuthorId = int.Parse(collection["Book.AuthorId"]!);
                existingBook.CategoryId = int.Parse(collection["Book.CategoryId"]!);
                existingBook.PublishedDate = DateTime.Parse(collection["Book.PublishedDate"]!);
                existingBook.ISBN = collection["Book.ISBN"];
                existingBook.CopiesAvailable = int.Parse(collection["Book.CopiesAvailable"]!);
                var updatedBook = await dbBooks.UpdateBookAsync(existingBook);
                TempData["Message"] = updatedBook is not null ? $"Book with ID {updatedBook.Id} updated successfully!" : "Failed to update book.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating the book.";
                return View();
            }
        }

        // GET: BooksController/Delete/5
        [RequirePermission(Permissions.DeleteBook)]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await dbBooks.GetBookByIdAsync(id);
            return View(book);
        }

        // POST: BooksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.DeleteBook)]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            if (await dbReservations.BookHasReservationsAsync(id))
            {
                TempData["Error"] = $"Book with ID {id} has active reservations and cannot be deleted.";
                return RedirectToAction(nameof(Index));
            }
            try
            {
                var deleted = await dbBooks.DeleteBookAsync(id);
                TempData["Message"] = deleted ? $"Book with ID {id} deleted successfully!" : $"Failed to delete book with ID {id}.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the book.";
                return View();
            }
        }
    }
}

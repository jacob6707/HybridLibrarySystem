using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Web.Features.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Controllers.Admin
{
    [Area("Admin")]
    public class AuthorsController(IAuthorRepository dbAuthors) : Controller
    {
        // GET: AuthorsController
        public async Task<IActionResult> Index()
        {
            var authors = await dbAuthors.GetAuthorSummariesAsync();
            return View(authors);
        }

        // GET: AuthorsController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var author = await dbAuthors.GetAuthorWithBooksByIdAsync(id);
            return View(author);
        }

        [RequirePermission(Permissions.CreateAuthor)]
        // GET: AuthorsController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AuthorsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission(Permissions.CreateAuthor)]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                // TODO: Implement the logic to create a new author
                var newAuthor = new HybridLibrarySystem.Core.Model.Author
                {
                    Name = collection["Name"],
                    Biography = collection["Biography"],
                    DateOfBirth = DateTime.Parse(collection["DateOfBirth"]!),
                    DateOfDeath = string.IsNullOrEmpty(collection["DateOfDeath"]) ? null : DateTime.Parse(collection["DateOfDeath"]!),
                    Nationality = collection["Nationality"]
                };
                var result = await dbAuthors.AddAuthorAsync(newAuthor);
                TempData["Message"] = result is not null ? $"Author with ID {result.Id} created successfully." : "Failed to create author.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while creating the author.";
                return View();
            }
        }

        // GET: AuthorsController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var author = await dbAuthors.GetAuthorByIdAsync(id);
            return View(author);
        }

        // POST: AuthorsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                var author = await dbAuthors.GetAuthorByIdAsync(id);
                if (author == null)
                {
                    return NotFound();
                }
                author.Name = collection["Name"];
                author.Biography = collection["Biography"];
                author.DateOfBirth = DateTime.Parse(collection["DateOfBirth"]!);
                author.DateOfDeath = string.IsNullOrEmpty(collection["DateOfDeath"]) ? null : DateTime.Parse(collection["DateOfDeath"]!);
                author.Nationality = collection["Nationality"];
                var result = await dbAuthors.UpdateAuthorAsync(author);
                TempData["Message"] = result is not null ? $"Author with ID {author.Id} updated successfully." : "Failed to update author.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating the author.";
                return View();
            }
        }

        // GET: AuthorsController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var author = await dbAuthors.GetAuthorWithBooksByIdAsync(id);
            return View(author);
        }

        // POST: AuthorsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var deleted = await dbAuthors.DeleteAuthorAsync(id);
                TempData["Message"] = deleted ? $"Author with ID {id} deleted successfully." : $"Failed to delete author with ID {id}.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the author.";
                return View();
            }
        }
    }
}

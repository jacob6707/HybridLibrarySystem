using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Controllers.Admin
{
    [Area("Admin")]
    public class CategoriesController(ICategoryRepository dbCategories) : Controller
    {
        // GET: CategoriesController
        public async Task<IActionResult> Index()
        {
            var categories = await dbCategories.GetAllCategoriesAsync();
            return View(categories);
        }

        // GET: CategoriesController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await dbCategories.GetCategoryWithBooksByIdAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        // GET: CategoriesController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CategoriesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                var category = new Category
                {
                    Name = collection["Name"]
                };
                var newCategory = await dbCategories.AddCategoryAsync(category);
                TempData["Message"] = newCategory is not null ? $"Category '{newCategory.Name}' created successfully." : "Failed to create category.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while creating the category.";
                return View();
            }
        }

        // GET: CategoriesController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await dbCategories.GetCategoryByIdAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: CategoriesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IFormCollection collection)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(collection);
                }
                var category = await dbCategories.GetCategoryByIdAsync(id);
                if (category is null)
                {
                    return NotFound();
                }
                category.Name = collection["Name"];
                var updatedCategory = await dbCategories.UpdateCategoryAsync(category);
                TempData["Message"] = updatedCategory is not null ? $"Category '{updatedCategory.Name}' updated successfully." : "Failed to update category.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while updating the category.";
                return View();
            }
        }

        // GET: CategoriesController/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await dbCategories.GetCategoryWithBooksByIdAsync(id);
            if (category is null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: CategoriesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                var deleted = await dbCategories.DeleteCategoryAsync(id);
                TempData["Message"] = deleted ? "Category deleted successfully." : "Failed to delete category.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "An error occurred while deleting the category.";
                return View();
            }
        }
    }
}

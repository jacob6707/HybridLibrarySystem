using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Core.Model.DTO;
using HybridLibrarySystem.Web.Features.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Api.V1;

public class CategoriesController(ICategoryRepository dbCategories) : V1ApiControllerBase
{
    [HttpGet]
    [RequirePermission(Permissions.ReadCategory)]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await dbCategories.GetAllCategoriesAsync();
        return Ok(categories.Select(c => new CategoryDTO(c.Id, c.Name)).ToList());
    }
}

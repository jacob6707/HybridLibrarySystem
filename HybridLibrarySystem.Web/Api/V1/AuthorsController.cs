using HybridLibrarySystem.Core.Interface;
using HybridLibrarySystem.Core.Model.Authorization;
using HybridLibrarySystem.Core.Model.DTO;
using HybridLibrarySystem.Web.Features.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HybridLibrarySystem.Web.Api.V1;

public class AuthorsController(IAuthorRepository dbAuthors) : V1ApiControllerBase
{
    [HttpGet]
    [RequirePermission(Permissions.ReadAuthor)]
    public async Task<IActionResult> GetAuthors()
    {
        var authors = await dbAuthors.GetAllAuthorsAsync();
        return Ok(authors.Select(a => new AuthorDTO(a.Id, a.Name, a.Biography, a.DateOfBirth, a.DateOfDeath, a.Nationality)).ToList());
    }
}

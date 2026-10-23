using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace HybridLibrarySystem.Core.Model;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

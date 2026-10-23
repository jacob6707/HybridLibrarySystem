using HybridLibrarySystem.Core.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HybridLibrarySystem.Web.ViewModel;
public class BookFormViewModel
{
    public Book? Book { get; set; }
    public SelectList? Authors { get; set; }
    public SelectList? Categories { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace HybridLibrarySystem.Core.Model.DTO;

public record BookDTO(
    int Id,
    string? Title,
    string? ISBN,
    AuthorDTO? Author,
    CategoryDTO? Category,
    [DataType(DataType.Date), Display(Name = "Publication Date")]
    DateTime PublishedDate,
    int CopiesAvailable
);

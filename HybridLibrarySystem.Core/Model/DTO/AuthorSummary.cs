using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HybridLibrarySystem.Core.Model.DTO;

public record AuthorSummary(
    int Id,
    string? Name,
    string? Biography,
    [DataType(DataType.Date), Display(Name = "Date of Birth")]
    DateTime DateOfBirth,
    [Display(Name = "Date of Death"), DataType(DataType.Date)]
    DateTime? DateOfDeath,
    string? Nationality,
    int BookCount
);
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HybridLibrarySystem.Core.Model
{
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        [Required, MaxLength(1000)]
        public string? Biography { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Date of Death"), DataType(DataType.Date)]
        public DateTime? DateOfDeath { get; set; }
        [Required, MaxLength(100)]
        public string? Nationality { get; set; }
        public ICollection<Book>? Books { get; set; }
    }
}

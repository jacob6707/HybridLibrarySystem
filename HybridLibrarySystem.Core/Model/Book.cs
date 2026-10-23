using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HybridLibrarySystem.Core.Model
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string? Title { get; set; }
        [Required, MaxLength(13)]
        public string? ISBN { get; set; }
        [Required]
        public int AuthorId { get; set; }
        public Author? Author { get; set; } = null!;
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null!;
        [Required, DataType(DataType.Date), Display(Name = "Date Published")]
        public DateTime PublishedDate { get; set; }
        [Required, Range(0, int.MaxValue), Display(Name = "Copies Available")]
        public int CopiesAvailable { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HybridLibrarySystem.Core.Model
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string? Name { get; set; }
        public ICollection<Book>? Books { get; set; } = null!;

    }
}

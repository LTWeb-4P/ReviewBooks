using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BECore.Models
{
    public class Book
    {
        public string Id { get; set; } = string.Empty; // Google Book ID
        public string Title { get; set; } = string.Empty;
        public string? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Review>? Reviews { get; set; }
    }
}
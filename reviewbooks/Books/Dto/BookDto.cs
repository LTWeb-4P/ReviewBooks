using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewBooks.Reviews.Models;

namespace ReviewBooks.Books.Dto
{
    public class BookDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }

        // Google Books ratings
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }

        // System ratings (from user reviews)
        public double? SystemAverageRating { get; set; }
        public int SystemRatingsCount { get; set; }
    }
    public class BookReview : BookDto
    {
        public List<Review>? Reviews { get; set; }

    }
}
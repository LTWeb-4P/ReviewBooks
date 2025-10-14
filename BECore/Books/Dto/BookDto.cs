using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BECore.Reviews.Models;

namespace BECore.Books.Dto
{
    public class BookDto
    {
        public Guid id { get; set; } 
        public string Title { get; set; }
        public string? Authors { get; set; }
        public string? Publisher { get; set; }
        public string? Description { get; set; }
        public string? Thumbnail { get; set; }
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }
    }
    public class BookReview : BookDto
    {
        public List<Review>? Reviews { get; set; }
        
    }
}
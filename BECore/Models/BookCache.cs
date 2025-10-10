using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BECore.Models
{
    public class BookCache
    {
        public string BookId { get; set; } = string.Empty;
        public string JsonData { get; set; } = string.Empty; // raw JSON Google Books
        public DateTime CachedAt { get; set; } = DateTime.UtcNow;
    }
}
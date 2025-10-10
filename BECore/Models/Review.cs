using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BECore.Models
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BookId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Book? Book { get; set; }
        public User? User { get; set; }
    }
}
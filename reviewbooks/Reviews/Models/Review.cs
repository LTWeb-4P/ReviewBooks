using ReviewBooks.Users.Models;
using ReviewBooks.Books.Models;
using AutoMapper;
using ReviewBooks.Reviews.Models;
using ReviewBooks.Reviews.Dto;

namespace ReviewBooks.Reviews.Models
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string BookId { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Book? Book { get; set; }
        public User? User { get; set; }
    }


}

namespace ReviewBooks.Reviews.Automapping
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewDetailDto>();
            CreateMap<AddReviewRequestDto, Review>();
            CreateMap<UpdateReviewRequestDto, Review>();
        }
    }
}
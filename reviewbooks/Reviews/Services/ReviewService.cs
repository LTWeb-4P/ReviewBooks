using ReviewBooks.Reviews.Repository;
using ReviewBooks.Reviews.Models;
using Shared;
using ReviewBooks.Reviews.Dto;
using AutoMapper;

namespace ReviewBooks.Reviews.Services
{

    public interface IReviewService
    {
        Task<PageResult<ReviewListDto>> GetReviewsAsync(Shared.Query query);
        Task<PageResult<ReviewDetailDto>> GetReviewsByBookIdAsync(string bookId, Shared.Query query);
        Task<PageResult<ReviewDetailDto>> GetReviewsByUserIdAsync(Guid userId, Shared.Query query);
        Task<ReviewDetailDto?> GetReviewByIdAsync(Guid id);
        Task<ReviewDetailDto> CreateReviewAsync(AddReviewRequestDto dto, Guid currentUserId);
        Task<ReviewDetailDto?> UpdateReviewAsync(Guid id, UpdateReviewRequestDto dto, Guid currentUserId, string userRole);
        Task<bool> DeleteReviewAsync(Guid id, Guid currentUserId, string userRole);
    }

    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ReviewDetailDto> CreateReviewAsync(AddReviewRequestDto dto, Guid currentUserId)
        {
            var reviewEntity = _mapper.Map<Review>(dto);
            reviewEntity.UserId = currentUserId; // Override with authenticated user
            reviewEntity.CreatedAt = DateTime.UtcNow;
            await _reviewRepository.CreateReviewAsync(reviewEntity);
            return _mapper.Map<ReviewDetailDto>(reviewEntity);
        }

        public async Task<bool> DeleteReviewAsync(Guid id, Guid currentUserId, string userRole)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) return false;

            // Role-based logic: Admin can delete any, User only their own
            if (userRole != "Admin" && review.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only delete your own reviews.");
            }

            return await _reviewRepository.DeleteReviewAsync(id);
        }

        public async Task<ReviewDetailDto?> GetReviewByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) return null;

            var dto = _mapper.Map<ReviewDetailDto>(review);
            dto.Username = review.User?.Username;
            dto.BookTitle = review.Book?.Title;
            return dto;
        }

        public async Task<PageResult<ReviewListDto>> GetReviewsAsync(Shared.Query query)
        {
            query.Normalize();

            var reviewsQuery = await _reviewRepository.GetReviewsQueryAsync();

            // Filter by search (BookId, Username, BookTitle)
            if (!string.IsNullOrEmpty(query.search))
            {
                var s = query.search.ToLower();
                reviewsQuery = reviewsQuery.Where(r =>
                    r.BookId.ToLower().Contains(s) ||
                    (r.User != null && r.User.Username.ToLower().Contains(s)) ||
                    (r.Book != null && r.Book.Title != null && r.Book.Title.ToLower().Contains(s)));
            }

            // Filter by author (userId)
            if (!string.IsNullOrEmpty(query.filterBy) && Guid.TryParse(query.filterBy, out var userId))
            {
                reviewsQuery = reviewsQuery.Where(r => r.UserId == userId);
            }

            // Sort
            reviewsQuery = query.sortBy?.ToLower() switch
            {
                "rating" => query.isDescending ? reviewsQuery.OrderByDescending(r => r.Rating) : reviewsQuery.OrderBy(r => r.Rating),
                "createdat" => query.isDescending ? reviewsQuery.OrderByDescending(r => r.CreatedAt) : reviewsQuery.OrderBy(r => r.CreatedAt),
                _ => reviewsQuery.OrderByDescending(r => r.CreatedAt)
            };

            var totalCount = reviewsQuery.Count();
            var items = reviewsQuery
                .Skip((query.pageNumber - 1) * query.pageSize)
                .Take(query.pageSize)
                .Select(r => new ReviewListDto
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    Username = r.User != null ? r.User.Username : null,
                    BookTitle = r.Book != null ? r.Book.Title : null
                })
                .ToList();

            return new PageResult<ReviewListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = query.pageSize,
                CurrentPage = query.pageNumber
            };
        }

        public async Task<PageResult<ReviewDetailDto>> GetReviewsByBookIdAsync(string bookId, Shared.Query query)
        {
            query.Normalize();

            var reviewsQuery = await _reviewRepository.GetReviewsQueryAsync();
            reviewsQuery = reviewsQuery.Where(r => r.BookId == bookId);

            // Sort
            reviewsQuery = query.sortBy?.ToLower() switch
            {
                "rating" => query.isDescending ? reviewsQuery.OrderByDescending(r => r.Rating) : reviewsQuery.OrderBy(r => r.Rating),
                "createdat" => query.isDescending ? reviewsQuery.OrderByDescending(r => r.CreatedAt) : reviewsQuery.OrderBy(r => r.CreatedAt),
                _ => reviewsQuery.OrderByDescending(r => r.CreatedAt)
            };

            var totalCount = reviewsQuery.Count();
            var items = reviewsQuery
                .Skip((query.pageNumber - 1) * query.pageSize)
                .Take(query.pageSize)
                .Select(r => new ReviewDetailDto
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    Username = r.User != null ? r.User.Username : null,
                    BookTitle = r.Book != null ? r.Book.Title : null
                })
                .ToList();

            return new PageResult<ReviewDetailDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = query.pageSize,
                CurrentPage = query.pageNumber
            };
        }

        public async Task<PageResult<ReviewDetailDto>> GetReviewsByUserIdAsync(Guid userId, Shared.Query query)
        {
            query.Normalize();

            var reviewsQuery = await _reviewRepository.GetReviewsQueryAsync();
            reviewsQuery = reviewsQuery.Where(r => r.UserId == userId);

            // Sort
            reviewsQuery = query.sortBy?.ToLower() switch
            {
                "rating" => query.isDescending ? reviewsQuery.OrderByDescending(r => r.Rating) : reviewsQuery.OrderBy(r => r.Rating),
                "createdat" => query.isDescending ? reviewsQuery.OrderByDescending(r => r.CreatedAt) : reviewsQuery.OrderBy(r => r.CreatedAt),
                _ => reviewsQuery.OrderByDescending(r => r.CreatedAt)
            };

            var totalCount = reviewsQuery.Count();
            var items = reviewsQuery
                .Skip((query.pageNumber - 1) * query.pageSize)
                .Take(query.pageSize)
                .Select(r => new ReviewDetailDto
                {
                    Id = r.Id,
                    BookId = r.BookId,
                    UserId = r.UserId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    Username = r.User != null ? r.User.Username : null,
                    BookTitle = r.Book != null ? r.Book.Title : null
                })
                .ToList();

            return new PageResult<ReviewDetailDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageSize = query.pageSize,
                CurrentPage = query.pageNumber
            };
        }

        public async Task<ReviewDetailDto?> UpdateReviewAsync(Guid id, UpdateReviewRequestDto dto, Guid currentUserId, string userRole)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null) return null;

            // Role-based logic: Admin can update any, User only their own
            if (userRole != "Admin" && review.UserId != currentUserId)
            {
                throw new UnauthorizedAccessException("You can only update your own reviews.");
            }

            if (dto.Rating.HasValue) review.Rating = dto.Rating.Value;
            if (dto.Comment != null) review.Comment = dto.Comment;
            review.UpdatedAt = DateTime.UtcNow;

            var updated = await _reviewRepository.UpdateReviewAsync(id, review);
            if (updated == null) return null;

            var result = _mapper.Map<ReviewDetailDto>(updated);
            result.Username = updated.User?.Username;
            result.BookTitle = updated.Book?.Title;
            return result;
        }
    }
}
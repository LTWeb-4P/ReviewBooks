using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReviewBooks.Reviews.Models;
using Shared;
using ReviewBooks.Data;
using Microsoft.EntityFrameworkCore;
using ReviewBooks.Users.Models;

namespace ReviewBooks.Reviews.Repository
{
    public interface IReviewRepository
    {
        Task<IQueryable<Review>> GetReviewsQueryAsync();
        Task<List<Review>> GetReviewsAsync();
        Task<IEnumerable<Review>> GetReviewByBookIdAsync(string bookId);
        Task<IEnumerable<Review>> GetReviewByUserIdAsync(Guid userId);
        Task<Review?> GetReviewByIdAsync(Guid id);
        Task<Review> CreateReviewAsync(Review review);
        Task<Review?> UpdateReviewAsync(Guid id, Review review);
        Task<bool> DeleteReviewAsync(Guid id);
    }

    public class ReviewRepository : IReviewRepository
    {

        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<Review>> GetReviewsQueryAsync()
        {
            return await Task.FromResult(_context.Reviews
                .Include(r => r.User)
                .Include(r => r.Book)
                .AsNoTracking());
        }

        public async Task<Review> CreateReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (review == null) return false;

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Review>> GetReviewByUserIdAsync(Guid userId)
        {
            var reviews = await _context.Reviews.Where(x => x.UserId == userId).ToListAsync();
            return reviews;
        }

        public async Task<Review?> GetReviewByIdAsync(Guid id)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            return review!;
        }

        public async Task<List<Review>> GetReviewsAsync()
        {
            var reviews = await _context.Reviews.Include(r => r.User).Include(r => r.Book).ToListAsync();
            return reviews;
        }

        public async Task<IEnumerable<Review>> GetReviewByBookIdAsync(string bookId)
        {
            var reviews = await _context.Reviews.Where(x => x.BookId == bookId).ToListAsync();
            return reviews;
        }

        public async Task<Review?> UpdateReviewAsync(Guid id, Review review)
        {
            var existingReview = await _context.Reviews.FirstOrDefaultAsync(x => x.Id == id);
            if (existingReview == null) return null;

            existingReview.Comment = review.Comment;
            existingReview.Rating = review.Rating;

            await _context.SaveChangesAsync();
            return existingReview;
        }
    }
}
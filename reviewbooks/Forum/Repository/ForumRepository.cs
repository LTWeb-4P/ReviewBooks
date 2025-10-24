using ReviewBooks.Data;
using ReviewBooks.Forum.Model;
using Microsoft.EntityFrameworkCore;

namespace ReviewBooks.Forum.Repository
{
    public class ForumRepository : IForumRepository
    {
        private readonly ApplicationDbContext _context;

        public ForumRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Posts
        public async Task<IEnumerable<ForumPost>> GetAllPostsAsync()
        {
            return await _context.ForumPosts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .OrderByDescending(p => p.IsPinned)
                .ThenByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<ForumPost?> GetPostByIdAsync(Guid id)
        {
            return await _context.ForumPosts
                .Include(p => p.User)
                .Include(p => p.Comments!)
                .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<ForumPost> CreatePostAsync(ForumPost post)
        {
            _context.ForumPosts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<ForumPost?> UpdatePostAsync(ForumPost post)
        {
            var existingPost = await _context.ForumPosts.FindAsync(post.Id);
            if (existingPost == null) return null;

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPost;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await _context.ForumPosts.FindAsync(id);
            if (post == null) return false;

            _context.ForumPosts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task IncrementViewCountAsync(Guid postId)
        {
            var post = await _context.ForumPosts.FindAsync(postId);
            if (post != null)
            {
                post.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        // Comments
        public async Task<IEnumerable<ForumComment>> GetCommentsByPostIdAsync(Guid postId)
        {
            return await _context.ForumComments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<ForumComment?> GetCommentByIdAsync(Guid id)
        {
            return await _context.ForumComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ForumComment> CreateCommentAsync(ForumComment comment)
        {
            _context.ForumComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<ForumComment?> UpdateCommentAsync(ForumComment comment)
        {
            var existingComment = await _context.ForumComments.FindAsync(comment.Id);
            if (existingComment == null) return null;

            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingComment;
        }

        public async Task<bool> DeleteCommentAsync(Guid id)
        {
            var comment = await _context.ForumComments.FindAsync(id);
            if (comment == null) return false;

            _context.ForumComments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

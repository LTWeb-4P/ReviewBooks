using ReviewBooks.Forum.Model;

namespace ReviewBooks.Forum.Repository
{
    public interface IForumRepository
    {
        // Posts
        Task<IEnumerable<ForumPost>> GetAllPostsAsync();
        Task<ForumPost?> GetPostByIdAsync(Guid id);
        Task<ForumPost> CreatePostAsync(ForumPost post);
        Task<ForumPost?> UpdatePostAsync(ForumPost post);
        Task<bool> DeletePostAsync(Guid id);
        Task IncrementViewCountAsync(Guid postId);

        // Comments
        Task<IEnumerable<ForumComment>> GetCommentsByPostIdAsync(Guid postId);
        Task<ForumComment?> GetCommentByIdAsync(Guid id);
        Task<ForumComment> CreateCommentAsync(ForumComment comment);
        Task<ForumComment?> UpdateCommentAsync(ForumComment comment);
        Task<bool> DeleteCommentAsync(Guid id);
    }
}

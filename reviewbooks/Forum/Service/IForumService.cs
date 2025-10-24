using ReviewBooks.Forum.Dto;
using Shared;

namespace ReviewBooks.Forum.Service
{
    public interface IForumService
    {
        // Posts
        Task<PageResult<ForumPostDto>> GetAllPostsAsync(Query query);
        Task<ForumPostDto?> GetPostByIdAsync(Guid id, bool incrementView = true);
        Task<ForumPostDto> CreatePostAsync(CreateForumPostDto dto, Guid userId);
        Task<ForumPostDto?> UpdatePostAsync(Guid id, UpdateForumPostDto dto, Guid currentUserId, string currentUserRole);
        Task<bool> DeletePostAsync(Guid id, Guid currentUserId, string currentUserRole);
        Task<ForumPostDto?> TogglePinPostAsync(Guid id, string currentUserRole);
        Task<ForumPostDto?> ToggleLockPostAsync(Guid id, string currentUserRole);

        // Comments
        Task<IEnumerable<ForumCommentDto>> GetCommentsByPostIdAsync(Guid postId);
        Task<ForumCommentDto> CreateCommentAsync(Guid postId, CreateForumCommentDto dto, Guid userId);
        Task<ForumCommentDto?> UpdateCommentAsync(Guid commentId, UpdateForumCommentDto dto, Guid currentUserId, string currentUserRole);
        Task<bool> DeleteCommentAsync(Guid commentId, Guid currentUserId, string currentUserRole);
    }
}

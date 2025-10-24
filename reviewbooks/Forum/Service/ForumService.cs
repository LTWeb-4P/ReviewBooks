using ReviewBooks.Forum.Dto;
using ReviewBooks.Forum.Model;
using ReviewBooks.Forum.Repository;
using Shared;

namespace ReviewBooks.Forum.Service
{
    public class ForumService : IForumService
    {
        private readonly IForumRepository _repository;

        public ForumService(IForumRepository repository)
        {
            _repository = repository;
        }

        // Posts
        public async Task<PageResult<ForumPostDto>> GetAllPostsAsync(Query query)
        {
            var posts = await _repository.GetAllPostsAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(query.search))
            {
                posts = posts.Where(p =>
                    p.Title.Contains(query.search, StringComparison.OrdinalIgnoreCase) ||
                    p.Content.Contains(query.search, StringComparison.OrdinalIgnoreCase));
            }

            // Apply sorting
            posts = query.sortBy?.ToLower() switch
            {
                "title" => query.isDescending ? posts.OrderByDescending(p => p.Title) : posts.OrderBy(p => p.Title),
                "views" => query.isDescending ? posts.OrderByDescending(p => p.ViewCount) : posts.OrderBy(p => p.ViewCount),
                "comments" => query.isDescending ? posts.OrderByDescending(p => p.Comments?.Count ?? 0) : posts.OrderBy(p => p.Comments?.Count ?? 0),
                _ => posts.OrderByDescending(p => p.IsPinned).ThenByDescending(p => p.CreatedAt)
            };

            var postDtos = posts.Select(p => new ForumPostDto
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                UserId = p.UserId,
                Username = p.User?.Username ?? "Unknown",
                UserAvatar = p.User?.AvatarUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ViewCount = p.ViewCount,
                CommentCount = p.Comments?.Count ?? 0,
                IsPinned = p.IsPinned,
                IsLocked = p.IsLocked
            });

            return PageResult<ForumPostDto>.Create(postDtos, query.pageNumber, query.pageSize);
        }

        public async Task<ForumPostDto?> GetPostByIdAsync(Guid id, bool incrementView = true)
        {
            if (incrementView)
            {
                await _repository.IncrementViewCountAsync(id);
            }

            var post = await _repository.GetPostByIdAsync(id);
            if (post == null) return null;

            return new ForumPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                Username = post.User?.Username ?? "Unknown",
                UserAvatar = post.User?.AvatarUrl,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                ViewCount = post.ViewCount,
                CommentCount = post.Comments?.Count ?? 0,
                IsPinned = post.IsPinned,
                IsLocked = post.IsLocked
            };
        }

        public async Task<ForumPostDto> CreatePostAsync(CreateForumPostDto dto, Guid userId)
        {
            var post = new ForumPost
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = userId
            };

            var created = await _repository.CreatePostAsync(post);
            var fullPost = await _repository.GetPostByIdAsync(created.Id);

            return new ForumPostDto
            {
                Id = fullPost!.Id,
                Title = fullPost.Title,
                Content = fullPost.Content,
                UserId = fullPost.UserId,
                Username = fullPost.User?.Username ?? "Unknown",
                UserAvatar = fullPost.User?.AvatarUrl,
                CreatedAt = fullPost.CreatedAt,
                UpdatedAt = fullPost.UpdatedAt,
                ViewCount = fullPost.ViewCount,
                CommentCount = 0,
                IsPinned = fullPost.IsPinned,
                IsLocked = fullPost.IsLocked
            };
        }

        public async Task<ForumPostDto?> UpdatePostAsync(Guid id, UpdateForumPostDto dto, Guid currentUserId, string currentUserRole)
        {
            var post = await _repository.GetPostByIdAsync(id);
            if (post == null) return null;

            // Authorization: User can only update own posts, Admin can update any
            if (post.UserId != currentUserId && currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("You can only update your own posts");
            }

            if (post.IsLocked && currentUserRole != "Admin")
            {
                throw new InvalidOperationException("This post is locked and cannot be edited");
            }

            if (!string.IsNullOrWhiteSpace(dto.Title))
                post.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.Content))
                post.Content = dto.Content;

            var updated = await _repository.UpdatePostAsync(post);
            if (updated == null) return null;

            return await GetPostByIdAsync(id, incrementView: false);
        }

        public async Task<bool> DeletePostAsync(Guid id, Guid currentUserId, string currentUserRole)
        {
            var post = await _repository.GetPostByIdAsync(id);
            if (post == null) return false;

            // Authorization: User can only delete own posts, Admin can delete any
            if (post.UserId != currentUserId && currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("You can only delete your own posts");
            }

            return await _repository.DeletePostAsync(id);
        }

        public async Task<ForumPostDto?> TogglePinPostAsync(Guid id, string currentUserRole)
        {
            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("Only admins can pin posts");
            }

            var post = await _repository.GetPostByIdAsync(id);
            if (post == null) return null;

            post.IsPinned = !post.IsPinned;
            await _repository.UpdatePostAsync(post);

            return await GetPostByIdAsync(id, incrementView: false);
        }

        public async Task<ForumPostDto?> ToggleLockPostAsync(Guid id, string currentUserRole)
        {
            if (currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("Only admins can lock posts");
            }

            var post = await _repository.GetPostByIdAsync(id);
            if (post == null) return null;

            post.IsLocked = !post.IsLocked;
            await _repository.UpdatePostAsync(post);

            return await GetPostByIdAsync(id, incrementView: false);
        }

        // Comments
        public async Task<IEnumerable<ForumCommentDto>> GetCommentsByPostIdAsync(Guid postId)
        {
            var comments = await _repository.GetCommentsByPostIdAsync(postId);
            return comments.Select(c => new ForumCommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                UserId = c.UserId,
                Username = c.User?.Username ?? "Unknown",
                UserAvatar = c.User?.AvatarUrl,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<ForumCommentDto> CreateCommentAsync(Guid postId, CreateForumCommentDto dto, Guid userId)
        {
            var post = await _repository.GetPostByIdAsync(postId);
            if (post == null)
            {
                throw new InvalidOperationException("Post not found");
            }

            if (post.IsLocked)
            {
                throw new InvalidOperationException("This post is locked and cannot accept new comments");
            }

            var comment = new ForumComment
            {
                PostId = postId,
                UserId = userId,
                Content = dto.Content
            };

            var created = await _repository.CreateCommentAsync(comment);
            var fullComment = await _repository.GetCommentByIdAsync(created.Id);

            return new ForumCommentDto
            {
                Id = fullComment!.Id,
                PostId = fullComment.PostId,
                UserId = fullComment.UserId,
                Username = fullComment.User?.Username ?? "Unknown",
                UserAvatar = fullComment.User?.AvatarUrl,
                Content = fullComment.Content,
                CreatedAt = fullComment.CreatedAt,
                UpdatedAt = fullComment.UpdatedAt
            };
        }

        public async Task<ForumCommentDto?> UpdateCommentAsync(Guid commentId, UpdateForumCommentDto dto, Guid currentUserId, string currentUserRole)
        {
            var comment = await _repository.GetCommentByIdAsync(commentId);
            if (comment == null) return null;

            // Authorization: User can only update own comments, Admin can update any
            if (comment.UserId != currentUserId && currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("You can only update your own comments");
            }

            var post = await _repository.GetPostByIdAsync(comment.PostId);
            if (post?.IsLocked == true && currentUserRole != "Admin")
            {
                throw new InvalidOperationException("This post is locked and comments cannot be edited");
            }

            comment.Content = dto.Content;
            var updated = await _repository.UpdateCommentAsync(comment);
            if (updated == null) return null;

            return new ForumCommentDto
            {
                Id = updated.Id,
                PostId = updated.PostId,
                UserId = updated.UserId,
                Username = updated.User?.Username ?? "Unknown",
                UserAvatar = updated.User?.AvatarUrl,
                Content = updated.Content,
                CreatedAt = updated.CreatedAt,
                UpdatedAt = updated.UpdatedAt
            };
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, Guid currentUserId, string currentUserRole)
        {
            var comment = await _repository.GetCommentByIdAsync(commentId);
            if (comment == null) return false;

            // Authorization: User can only delete own comments, Admin can delete any
            if (comment.UserId != currentUserId && currentUserRole != "Admin")
            {
                throw new UnauthorizedAccessException("You can only delete your own comments");
            }

            return await _repository.DeleteCommentAsync(commentId);
        }
    }
}

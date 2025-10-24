namespace ReviewBooks.Forum.Dto
{
    // Post DTOs
    public class ForumPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int CommentCount { get; set; }
        public bool IsPinned { get; set; }
        public bool IsLocked { get; set; }
    }

    public class CreateForumPostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class UpdateForumPostDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
    }

    // Comment DTOs
    public class ForumCommentDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateForumCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }

    public class UpdateForumCommentDto
    {
        public string Content { get; set; } = string.Empty;
    }
}

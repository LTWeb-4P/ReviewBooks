using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReviewBooks.Forum.Service;
using ReviewBooks.Forum.Dto;
using Shared;
using System.Security.Claims;

namespace ReviewBooks.Forum.Controller
{
    [ApiController]
    [Route("api/forum")]
    public class ForumController : ControllerBase
    {
        private readonly IForumService _forumService;

        public ForumController(IForumService forumService)
        {
            _forumService = forumService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        private string GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role) ?? "User";
        }

        #region Posts

        /// <summary>
        /// Get all forum posts with pagination and search
        /// </summary>
        [HttpGet("posts")]
        public async Task<ActionResult<PageResult<ForumPostDto>>> GetAllPosts([FromQuery] Query query)
        {
            try
            {
                var posts = await _forumService.GetAllPostsAsync(query);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get post by ID (increments view count)
        /// </summary>
        [HttpGet("posts/{id:guid}")]
        public async Task<ActionResult<ForumPostDto>> GetPostById(Guid id)
        {
            try
            {
                var post = await _forumService.GetPostByIdAsync(id);
                if (post == null) return NotFound(new { message = $"Post with id {id} not found" });
                return Ok(post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new forum post (requires authentication)
        /// </summary>
        [Authorize]
        [HttpPost("posts")]
        public async Task<ActionResult<ForumPostDto>> CreatePost([FromBody] CreateForumPostDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var post = await _forumService.CreatePostAsync(dto, userId);
                return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update forum post (User can update own, Admin can update any)
        /// </summary>
        [Authorize]
        [HttpPut("posts/{id:guid}")]
        public async Task<ActionResult<ForumPostDto>> UpdatePost(Guid id, [FromBody] UpdateForumPostDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var post = await _forumService.UpdatePostAsync(id, dto, userId, role);
                if (post == null) return NotFound(new { message = $"Post with id {id} not found" });
                return Ok(post);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete forum post (User can delete own, Admin can delete any)
        /// </summary>
        [Authorize]
        [HttpDelete("posts/{id:guid}")]
        public async Task<ActionResult> DeletePost(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var deleted = await _forumService.DeletePostAsync(id, userId, role);
                if (!deleted) return NotFound(new { message = $"Post with id {id} not found" });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Pin/Unpin post (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("posts/{id:guid}/pin")]
        public async Task<ActionResult<ForumPostDto>> TogglePin(Guid id)
        {
            try
            {
                var role = GetCurrentUserRole();
                var post = await _forumService.TogglePinPostAsync(id, role);
                if (post == null) return NotFound(new { message = $"Post with id {id} not found" });
                return Ok(post);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Lock/Unlock post (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPatch("posts/{id:guid}/lock")]
        public async Task<ActionResult<ForumPostDto>> ToggleLock(Guid id)
        {
            try
            {
                var role = GetCurrentUserRole();
                var post = await _forumService.ToggleLockPostAsync(id, role);
                if (post == null) return NotFound(new { message = $"Post with id {id} not found" });
                return Ok(post);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Comments

        /// <summary>
        /// Get all comments for a post
        /// </summary>
        [HttpGet("posts/{postId:guid}/comments")]
        public async Task<ActionResult<IEnumerable<ForumCommentDto>>> GetCommentsByPostId(Guid postId)
        {
            try
            {
                var comments = await _forumService.GetCommentsByPostIdAsync(postId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new comment (requires authentication)
        /// </summary>
        [Authorize]
        [HttpPost("posts/{postId:guid}/comments")]
        public async Task<ActionResult<ForumCommentDto>> CreateComment(Guid postId, [FromBody] CreateForumCommentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var comment = await _forumService.CreateCommentAsync(postId, dto, userId);
                return CreatedAtAction(nameof(GetCommentsByPostId), new { postId }, comment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update comment (User can update own, Admin can update any)
        /// </summary>
        [Authorize]
        [HttpPut("comments/{id:guid}")]
        public async Task<ActionResult<ForumCommentDto>> UpdateComment(Guid id, [FromBody] UpdateForumCommentDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var comment = await _forumService.UpdateCommentAsync(id, dto, userId, role);
                if (comment == null) return NotFound(new { message = $"Comment with id {id} not found" });
                return Ok(comment);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete comment (User can delete own, Admin can delete any)
        /// </summary>
        [Authorize]
        [HttpDelete("comments/{id:guid}")]
        public async Task<ActionResult> DeleteComment(Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var deleted = await _forumService.DeleteCommentAsync(id, userId, role);
                if (!deleted) return NotFound(new { message = $"Comment with id {id} not found" });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion
    }
}

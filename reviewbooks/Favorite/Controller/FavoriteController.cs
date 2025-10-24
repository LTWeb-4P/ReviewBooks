using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReviewBooks.Favorite.Services;
using ReviewBooks.Favorite.Dto;
using System.Security.Claims;

namespace ReviewBooks.Favorite.Controller
{
    [ApiController]
    [Route("api/favorites")]
    [Authorize] // All endpoints require authentication
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoriteController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        /// <summary>
        /// Get current user's favorite books
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteBookDto>>> GetMyFavorites()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Add book to favorites
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> AddFavorite([FromBody] AddFavoriteDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var added = await _favoriteService.AddFavoriteAsync(userId, dto.BookId);
                if (!added)
                {
                    return BadRequest(new { message = "Book is already in favorites or could not be added" });
                }

                return Ok(new { message = "Book added to favorites successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove book from favorites
        /// </summary>
        [HttpDelete("{bookId}")]
        public async Task<ActionResult> RemoveFavorite(string bookId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var removed = await _favoriteService.RemoveFavoriteAsync(userId, bookId);
                if (!removed)
                {
                    return NotFound(new { message = "Book not found in favorites" });
                }

                return Ok(new { message = "Book removed from favorites successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if book is in favorites
        /// </summary>
        [HttpGet("check/{bookId}")]
        public async Task<ActionResult<bool>> IsFavorite(string bookId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var isFavorite = await _favoriteService.IsFavoriteAsync(userId, bookId);
                return Ok(new { bookId, isFavorite });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewBooks.Reviews.Dto;
using ReviewBooks.Reviews.Services;
using Shared;
using System.Security.Claims;

namespace ReviewBooks.Reviews.Controller
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
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

        [HttpGet]
        public async Task<ActionResult<PageResult<ReviewListDto>>> GetReviews([FromQuery] Query query)
        {
            try
            {
                var result = await _reviewService.GetReviewsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<PageResult<ReviewDetailDto>>> GetReviewsByBook([FromRoute] string bookId, [FromQuery] Query query)
        {
            try
            {
                var result = await _reviewService.GetReviewsByBookIdAsync(bookId, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<PageResult<ReviewDetailDto>>> GetReviewsByUser([FromRoute] Guid userId, [FromQuery] Query query)
        {
            try
            {
                var result = await _reviewService.GetReviewsByUserIdAsync(userId, query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReviewDetailDto>> GetReviewById([FromRoute] Guid id)
        {
            try
            {
                var result = await _reviewService.GetReviewByIdAsync(id);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDetailDto>> CreateReview([FromBody] AddReviewRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Invalid user token" });

                var result = await _reviewService.CreateReviewAsync(dto, userId);
                return CreatedAtAction(nameof(GetReviewById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ReviewDetailDto>> UpdateReview([FromRoute] Guid id, [FromBody] UpdateReviewRequestDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Invalid user token" });

                var result = await _reviewService.UpdateReviewAsync(id, dto, userId, role);
                if (result == null) return NotFound();
                return Ok(result);
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

        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteReview([FromRoute] Guid id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "Invalid user token" });

                var success = await _reviewService.DeleteReviewAsync(id, userId, role);
                if (!success) return NotFound();
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
    }
}
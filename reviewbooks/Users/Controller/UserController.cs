using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReviewBooks.Users.Services;
using ReviewBooks.Users.Dto;
using Shared;
using System.Security.Claims;

namespace ReviewBooks.Users.Controller
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
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

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<PageResult<UserDto>>> GetAllUsers([FromQuery] Query query)
        {
            try
            {
                var role = GetCurrentUserRole();
                var users = await _userService.GetAllUsersAsync(query, role);
                return Ok(users);
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
        /// Get user by ID (User can view own profile, Admin can view any)
        /// </summary>
        [Authorize]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var user = await _userService.GetUserByIdAsync(id, currentUserId, role);
                if (user == null) return NotFound(new { message = $"User with id {id} not found" });
                return Ok(user);
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
        /// Get current user profile
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetMe()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var user = await _userService.GetUserByIdAsync(currentUserId, currentUserId, role);
                if (user == null) return NotFound(new { message = "User not found" });
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update user profile (User can update own profile, Admin can update any)
        /// </summary>
        [Authorize]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UserDto>> UpdateUserProfile(Guid id, [FromBody] UpdateUserProfileDto dto)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var updatedUser = await _userService.UpdateUserProfileAsync(id, dto, currentUserId, role);
                if (updatedUser == null) return NotFound(new { message = $"User with id {id} not found" });
                return Ok(updatedUser);
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
        /// Admin update user (can change email, role, etc.) - Admin only
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/{id:guid}")]
        public async Task<ActionResult<UserDto>> AdminUpdateUser(Guid id, [FromBody] AdminUpdateUserDto dto)
        {
            try
            {
                var role = GetCurrentUserRole();
                var updatedUser = await _userService.AdminUpdateUserAsync(id, dto, role);
                if (updatedUser == null) return NotFound(new { message = $"User with id {id} not found" });
                return Ok(updatedUser);
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
        /// Delete user (User can delete own account, Admin can delete any)
        /// </summary>
        [Authorize]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var role = GetCurrentUserRole();
                var deleted = await _userService.DeleteUserAsync(id, currentUserId, role);
                if (!deleted) return NotFound(new { message = $"User with id {id} not found" });
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

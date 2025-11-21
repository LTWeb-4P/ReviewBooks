using Microsoft.AspNetCore.Mvc;
using ReviewBooks.Auth.Dto;
using ReviewBooks.Auth.Services;

namespace ReviewBooks.Auth.Controller
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        /// <summary>
        /// Verify email - User click vào link trong email
        /// </summary>
        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
        {
            try
            {
                var loginResponse = await _authService.VerifyEmailAsync(userId, token);

                if (loginResponse != null)
                {
                    // Redirect to frontend homepage after successful verification
                    var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
                    return Redirect(frontendUrl);
                }
                else
                {
                    // Redirect to frontend homepage even if verification fails
                    var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
                    return Redirect(frontendUrl);
                }
            }
            catch (Exception)
            {
                // Redirect to frontend homepage if error occurs
                var frontendUrl = _configuration["App:FrontendUrl"] ?? "http://localhost:3000";
                return Redirect(frontendUrl);
            }
        }

        /// <summary>
        /// Resend verification email
        /// </summary>
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
        {
            try
            {
                var result = await _authService.ResendVerificationEmailAsync(dto.Email);
                if (result)
                {
                    return Ok(new { message = "Verification email sent successfully" });
                }
                return BadRequest(new { message = "Email not found or already verified" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send verification email", error = ex.Message });
            }
        }


        /// <summary>
        /// Register a new user account - Send verification email
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                var message = await _authService.RegisterAsync(dto);
                return Ok(new { message });
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
        /// Login with email and password
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                if (result == null)
                {
                    return Unauthorized(new { message = "Invalid email or password" });
                }
                return Ok(result);
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
    }
}

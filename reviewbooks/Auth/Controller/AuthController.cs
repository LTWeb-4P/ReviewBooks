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
                    // Return HTML page with auto-redirect to Swagger
                    var backendUrl = _configuration["App:BackendUrl"] ?? "http://localhost:8080";
                    var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Email Verified</title>
    <style>
        body {{ font-family: Arial; text-align: center; padding: 50px; }}
        .success {{ color: #28a745; }}
        .token {{ background: #f5f5f5; padding: 15px; margin: 20px; word-break: break-all; }}
        button {{ background: #007bff; color: white; padding: 10px 20px; border: none; cursor: pointer; font-size: 16px; }}
    </style>
</head>
<body>
    <h1 class='success'>✓ Email Verified Successfully!</h1>
    <p>Your JWT Token:</p>
    <div class='token'>{loginResponse.Token}</div>
    <p>Copy the token above and use it in Swagger Authorization</p>
    <button onclick=""window.location.href='{backendUrl}/swagger/index.html'"">Go to Swagger</button>
</body>
</html>";
                    return Content(html, "text/html");
                }
                else
                {
                    var html = @"
<!DOCTYPE html>
<html>
<head><title>Verification Failed</title></head>
<body style='font-family: Arial; text-align: center; padding: 50px;'>
    <h1 style='color: #dc3545;'>✗ Verification Failed</h1>
    <p>Invalid or expired verification link</p>
</body>
</html>";
                    return Content(html, "text/html");
                }
            }
            catch (Exception)
            {
                var html = @"
<!DOCTYPE html>
<html>
<head><title>Error</title></head>
<body style='font-family: Arial; text-align: center; padding: 50px;'>
    <h1 style='color: #dc3545;'>✗ Verification Failed</h1>
    <p>An error occurred during verification</p>
</body>
</html>";
                return Content(html, "text/html");
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

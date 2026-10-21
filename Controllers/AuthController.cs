using AssetManagementAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Session3Context _context;

        public AuthController(Session3Context context)
        {
            _context = context;
        }

        // POST api/Auth/login
        // Body: { "username": "...", "password": "..." }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Tài khoản và mật khẩu không được để trống." });

            // Tìm user trong DB (so sánh username + password)
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.Password == request.Password);

            if (user == null)
                return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không chính xác." });

            return Ok(new LoginResponse
            {
                Message = "Đăng nhập thành công.",
                FullName = user.FullName ?? user.Username
            });
        }
    }

    // DTO nhận từ Android
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // DTO trả về cho Android
    public class LoginResponse
    {
        public string Message { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}

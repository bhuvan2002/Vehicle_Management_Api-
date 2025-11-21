using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.DTOs;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.Services;

namespace VehicleManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, IJwtService jwtService, ILogger<AuthController> logger)
        {
            _context = context;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            _logger.LogInformation($"Login attempt for email: {request.Email}");

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                _logger.LogWarning($"User not found with email: {request.Email}");
                return Unauthorized("Invalid email or password");
            }

            _logger.LogInformation($"User found: {user.Email}");

            // Direct string comparison (no hashing)
            bool isPasswordValid = user.PasswordHash == request.Password;
            _logger.LogInformation($"Password verification result: {isPasswordValid}");

            if (!isPasswordValid)
            {
                _logger.LogWarning($"Invalid password for user: {request.Email}");
                return Unauthorized("Invalid email or password");
            }

            var token = _jwtService.GenerateToken(user);
            _logger.LogInformation($"Login successful for user: {user.Email}");

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = user.UserId,
                Role = user.Role.RoleName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            });
        }
    }
}
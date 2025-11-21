using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.DTOs;
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Controllers
{
    [ApiController]
    [Route("api/my")]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }
            return int.Parse(userIdClaim);
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            try
            {
                var userId = GetCurrentUserId();

                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserId == userId);

                if (user == null)
                {
                    return NotFound("User not found");
                }

                var userDto = new UserDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Email = user.Email,
                    Role = user.Role.RoleName
                };

                return Ok(userDto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("vehicles")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMyVehicles()
        {
            try
            {
                var userId = GetCurrentUserId();

                var vehicles = await _context.Vehicles
                    .Where(v => v.AssignedToUserId == userId)
                    .Select(v => new VehicleDto
                    {
                        Id = v.Id,
                        VehicleNumber = v.VehicleNumber,
                        Brand = v.Brand,
                        Model = v.Model,
                        CurrentChargePercentage = v.CurrentChargePercentage,
                        MaxPayloadKg = v.MaxPayloadKg,
                        AssignStatus = v.AssignStatus.ToString()
                    })
                    .ToListAsync();

                return Ok(vehicles);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
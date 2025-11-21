using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet("profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {
            var user = (User)HttpContext.Items["User"]!;

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

        [HttpGet("vehicles")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetMyVehicles()
        {
            var user = (User)HttpContext.Items["User"]!;

            var vehicles = await _context.Vehicles
                .Where(v => v.AssignedToUserId == user.UserId)
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    CurrentChargePercentage = v.CurrentChargePercentage,
                    MaxPayloadKg = v.MaxPayloadKg,
                    ChargingStatus = v.ChargingStatus.ToString(),
                    AssignStatus = v.AssignStatus.ToString()
                })
                .ToListAsync();

            return Ok(vehicles);
        }
    }
}
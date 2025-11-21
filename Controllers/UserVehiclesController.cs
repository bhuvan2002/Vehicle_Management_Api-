using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [ApiController]
    [Route("api/users/{userId}/vehicles")]
    [Authorize(Roles = "admin")]
    public class UserVehiclesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserVehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetUserVehicles(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }

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
                    ChargingStatus = v.ChargingStatus.ToString(),
                    AssignStatus = v.AssignStatus.ToString()
                })
                .ToListAsync();

            return Ok(vehicles);
        }
    }
}
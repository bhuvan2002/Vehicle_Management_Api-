using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.DTOs;
using VehicleManagementAPI.Models;

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
                    AssignStatus = v.AssignStatus.ToString()
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        [HttpPost("{id}/assign")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AssignVehicle(int id, AssignVehicleRequest request)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found");
            }

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (vehicle.AssignStatus == AssignStatus.Assigned)
            {
                return BadRequest("Vehicle is already assigned");
            }

            vehicle.AssignedToUserId = request.UserId;
            vehicle.AssignStatus = AssignStatus.Assigned;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Vehicle assigned successfully" });
        }

        [HttpPost("{id}/unassign")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UnassignVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound("Vehicle not found");
            }

            if (vehicle.AssignStatus != AssignStatus.Assigned)
            {
                return BadRequest("Vehicle is not assigned");
            }

            vehicle.AssignedToUserId = null;
            vehicle.AssignStatus = AssignStatus.Available;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Vehicle unassigned successfully" });
        }
    }
}
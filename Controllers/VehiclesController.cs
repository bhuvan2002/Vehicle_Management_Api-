using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.DTOs;
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehiclesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetVehicles()
        {
            var vehicles = await _context.Vehicles
                .Include(v => v.AssignedToUser)
                .Select(v => new VehicleDto
                {
                    Id = v.Id,
                    VehicleNumber = v.VehicleNumber,
                    Brand = v.Brand,
                    Model = v.Model,
                    CurrentChargePercentage = v.CurrentChargePercentage,
                    MaxPayloadKg = v.MaxPayloadKg,
                    ChargingStatus = v.ChargingStatus.ToString(),
                    AssignStatus = v.AssignStatus.ToString(),
                    AssignedToUserId = v.AssignedToUserId,
                    AssignedToUserName = v.AssignedToUser != null ?
                        $"{v.AssignedToUser.FirstName} {v.AssignedToUser.LastName}" : null
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VehicleDto>> GetVehicle(int id)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.AssignedToUser)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (vehicle == null)
            {
                return NotFound();
            }

            var vehicleDto = new VehicleDto
            {
                Id = vehicle.Id,
                VehicleNumber = vehicle.VehicleNumber,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                CurrentChargePercentage = vehicle.CurrentChargePercentage,
                MaxPayloadKg = vehicle.MaxPayloadKg,
                ChargingStatus = vehicle.ChargingStatus.ToString(),
                AssignStatus = vehicle.AssignStatus.ToString(),
                AssignedToUserId = vehicle.AssignedToUserId,
                AssignedToUserName = vehicle.AssignedToUser != null ?
                    $"{vehicle.AssignedToUser.FirstName} {vehicle.AssignedToUser.LastName}" : null
            };

            return Ok(vehicleDto);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<VehicleDto>> CreateVehicle(CreateVehicleRequest request)
        {
            var vehicle = new Vehicle
            {
                VehicleNumber = request.VehicleNumber,
                Brand = request.Brand,
                Model = request.Model,
                CurrentChargePercentage = request.CurrentChargePercentage,
                MaxPayloadKg = request.MaxPayloadKg,
                ChargingStatus = request.ChargingStatus,
                AssignStatus = AssignStatus.Available
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            var vehicleDto = new VehicleDto
            {
                Id = vehicle.Id,
                VehicleNumber = vehicle.VehicleNumber,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                CurrentChargePercentage = vehicle.CurrentChargePercentage,
                MaxPayloadKg = vehicle.MaxPayloadKg,
                ChargingStatus = vehicle.ChargingStatus.ToString(),
                AssignStatus = vehicle.AssignStatus.ToString()
            };

            return CreatedAtAction(nameof(GetVehicle), new { id = vehicle.Id }, vehicleDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVehicle(int id, UpdateVehicleRequest request)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.VehicleNumber = request.VehicleNumber;
            vehicle.Brand = request.Brand;
            vehicle.Model = request.Model;
            vehicle.CurrentChargePercentage = request.CurrentChargePercentage;
            vehicle.MaxPayloadKg = request.MaxPayloadKg;
            vehicle.ChargingStatus = request.ChargingStatus;
            vehicle.AssignStatus = request.AssignStatus;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();

            return NoContent();
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
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal CurrentChargePercentage { get; set; }
        public decimal MaxPayloadKg { get; set; }
        public string ChargingStatus { get; set; } = string.Empty;
        public string AssignStatus { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
    }

    public class CreateVehicleRequest
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal CurrentChargePercentage { get; set; }
        public decimal MaxPayloadKg { get; set; }
        public ChargingStatus ChargingStatus { get; set; }
    }

    public class UpdateVehicleRequest
    {
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal CurrentChargePercentage { get; set; }
        public decimal MaxPayloadKg { get; set; }
        public ChargingStatus ChargingStatus { get; set; }
        public AssignStatus AssignStatus { get; set; }
    }

    public class AssignVehicleRequest
    {
        public int UserId { get; set; }
    }
}
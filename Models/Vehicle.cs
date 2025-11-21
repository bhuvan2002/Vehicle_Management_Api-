namespace VehicleManagementAPI.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public decimal CurrentChargePercentage { get; set; }
        public decimal MaxPayloadKg { get; set; }
        public AssignStatus AssignStatus { get; set; }

        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
    }

    public enum AssignStatus
    {
        Available,
        Assigned,
        Maintenance
    }
}
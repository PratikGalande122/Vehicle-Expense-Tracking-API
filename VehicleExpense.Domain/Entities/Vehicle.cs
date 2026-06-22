using VehicleExpense.Domain.Common;
using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Domain.Entities;

/// <summary>
/// Represents a vehicle owned by a user.
/// </summary>
public class Vehicle : BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public FuelType FuelType { get; set; }
    public int Year { get; set; }
    public string? Brand { get; set; }
    public string? Model { get; set; }
}

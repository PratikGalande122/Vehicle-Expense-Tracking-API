using VehicleExpense.Domain.Common;

namespace VehicleExpense.Domain.Entities;

/// <summary>
/// Represents a fuel fill-up log for a vehicle.
/// </summary>
public class FuelLog : BaseEntity
{
    public int VehicleId { get; set; }
    public int UserId { get; set; }
    public decimal LitresFilled { get; set; }
    public decimal PricePerLitre { get; set; }
    public decimal TotalCost { get; set; }
    public decimal OdometerReading { get; set; }
    public DateTime FilledAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

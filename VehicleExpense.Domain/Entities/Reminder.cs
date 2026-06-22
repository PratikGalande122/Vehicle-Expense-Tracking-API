using VehicleExpense.Domain.Common;

namespace VehicleExpense.Domain.Entities;

/// <summary>
/// Represents a maintenance or insurance reminder for a vehicle.
/// </summary>
public class Reminder : BaseEntity
{
    public int VehicleId { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsCompleted { get; set; } = false;
}

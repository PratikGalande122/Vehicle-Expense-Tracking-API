namespace VehicleExpense.Domain.Common;

/// <summary>
/// Base class for all entities with a common identifier and audit fields.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

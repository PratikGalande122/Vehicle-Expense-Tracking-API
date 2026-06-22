using VehicleExpense.Domain.Common;
using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Domain.Entities;

/// <summary>
/// Represents an expense incurred for a vehicle (e.g., service, insurance).
/// </summary>
public class Expense : BaseEntity
{
    public int VehicleId { get; set; }
    public int UserId { get; set; }
    public ExpenseType ExpenseType { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime ExpenseDate { get; set; } = DateTime.UtcNow;
    public string? Place { get; set; }
    public string? Driver { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Reason { get; set; }
}

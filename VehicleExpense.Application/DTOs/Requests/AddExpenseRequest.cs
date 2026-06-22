using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Request payload for logging a vehicle expense.</summary>
public record AddExpenseRequest(
    int VehicleId,
    ExpenseType ExpenseType,
    decimal Amount,
    string? Description,
    DateTime ExpenseDate,
    string? Place,
    string? Driver,
    string? PaymentMethod,
    string? Reason
);

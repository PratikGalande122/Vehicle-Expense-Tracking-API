using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Request payload for editing an existing vehicle expense.</summary>
public record UpdateExpenseRequest(
    ExpenseType ExpenseType,
    decimal Amount,
    string? Description,
    DateTime ExpenseDate,
    string? Place,
    string? Driver,
    string? PaymentMethod,
    string? Reason
);

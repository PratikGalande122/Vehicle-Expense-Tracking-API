using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Business operations for expense management.
/// </summary>
public interface IExpenseService
{
    Task<IEnumerable<Expense>> GetExpensesAsync(int userId);
    Task<Expense?> AddExpenseAsync(int userId, AddExpenseRequest request);
    Task<Expense?> UpdateExpenseAsync(int userId, int id, UpdateExpenseRequest request);
    /// <summary>Deletes an expense owned by the user. Returns false if not found or not owned.</summary>
    Task<bool> DeleteExpenseAsync(int userId, int id);
}

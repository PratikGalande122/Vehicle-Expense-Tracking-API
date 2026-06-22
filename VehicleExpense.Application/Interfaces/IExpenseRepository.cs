using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

public interface IExpenseRepository
{
    Task<IEnumerable<Expense>> GetByUserIdAsync(int userId);
    Task<Expense?> GetByIdAsync(int id, int userId);
    Task<int> CreateAsync(Expense expense);
    Task<bool> UpdateAsync(Expense expense);
    /// <summary>Deletes an expense only if it belongs to the given user. Returns true if deleted.</summary>
    Task<bool> DeleteAsync(int id, int userId);
}

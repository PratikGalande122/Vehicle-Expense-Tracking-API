using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Services;

/// <summary>
/// Handles expense creation and retrieval for a user's vehicles.
/// </summary>
public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseService(IExpenseRepository expenseRepository)
    {
        _expenseRepository = expenseRepository;
    }

    public async Task<IEnumerable<Expense>> GetExpensesAsync(int userId) =>
        await _expenseRepository.GetByUserIdAsync(userId);

    public async Task<Expense?> AddExpenseAsync(int userId, AddExpenseRequest request)
    {
        var expense = new Expense
        {
            UserId = userId,
            VehicleId = request.VehicleId,
            ExpenseType = request.ExpenseType,
            Amount = request.Amount,
            Description = request.Description,
            ExpenseDate = request.ExpenseDate,
            Place = request.Place,
            Driver = request.Driver,
            PaymentMethod = request.PaymentMethod,
            Reason = request.Reason
        };

        var id = await _expenseRepository.CreateAsync(expense);
        expense.Id = id;
        return expense;
    }

    public async Task<Expense?> UpdateExpenseAsync(int userId, int id, UpdateExpenseRequest request)
    {
        var expense = await _expenseRepository.GetByIdAsync(id, userId);
        if (expense is null)
            return null;

        expense.ExpenseType = request.ExpenseType;
        expense.Amount = request.Amount;
        expense.Description = request.Description;
        expense.ExpenseDate = request.ExpenseDate;
        expense.Place = request.Place;
        expense.Driver = request.Driver;
        expense.PaymentMethod = request.PaymentMethod;
        expense.Reason = request.Reason;
        expense.UpdatedAt = DateTime.UtcNow;

        await _expenseRepository.UpdateAsync(expense);
        return expense;
    }

    public async Task<bool> DeleteExpenseAsync(int userId, int id) =>
        await _expenseRepository.DeleteAsync(id, userId);
}

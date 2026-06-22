using Dapper;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;
using VehicleExpense.Infrastructure.Data;

namespace VehicleExpense.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly DbConnectionFactory _factory;

    public ExpenseRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Expense>> GetByUserIdAsync(int userId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Expense>(
            "SELECT * FROM Expenses WHERE UserId = @UserId ORDER BY ExpenseDate DESC", new { UserId = userId });
    }

    public async Task<Expense?> GetByIdAsync(int id, int userId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Expense>(
            "SELECT * FROM Expenses WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
    }

    public async Task<int> CreateAsync(Expense expense)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            INSERT INTO Expenses (VehicleId, UserId, ExpenseType, Amount, Description, ExpenseDate, Place, Driver, PaymentMethod, Reason, CreatedAt, UpdatedAt)
            VALUES (@VehicleId, @UserId, @ExpenseType, @Amount, @Description, @ExpenseDate, @Place, @Driver, @PaymentMethod, @Reason, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();";
        return await conn.ExecuteScalarAsync<int>(sql, expense);
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            UPDATE Expenses
            SET ExpenseType  = @ExpenseType,
                Amount       = @Amount,
                Description  = @Description,
                ExpenseDate  = @ExpenseDate,
                Place        = @Place,
                Driver       = @Driver,
                PaymentMethod = @PaymentMethod,
                Reason       = @Reason,
                UpdatedAt    = @UpdatedAt
            WHERE Id = @Id AND UserId = @UserId";
        var affected = await conn.ExecuteAsync(sql, expense);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = _factory.CreateConnection();
        var affected = await conn.ExecuteAsync(
            "DELETE FROM Expenses WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
        return affected > 0;
    }
}

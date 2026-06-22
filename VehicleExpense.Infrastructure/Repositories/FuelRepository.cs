using Dapper;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;
using VehicleExpense.Infrastructure.Data;

namespace VehicleExpense.Infrastructure.Repositories;

public class FuelRepository : IFuelRepository
{
    private readonly DbConnectionFactory _factory;

    public FuelRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<FuelLog>> GetByUserIdAsync(int userId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<FuelLog>(
            "SELECT * FROM FuelLogs WHERE UserId = @UserId ORDER BY FilledAt DESC", new { UserId = userId });
    }

    public async Task<FuelLog?> GetByIdAsync(int id, int userId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<FuelLog>(
            "SELECT * FROM FuelLogs WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
    }

    public async Task<int> CreateAsync(FuelLog fuelLog)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            INSERT INTO FuelLogs (VehicleId, UserId, LitresFilled, PricePerLitre, TotalCost, OdometerReading, FilledAt, Notes, CreatedAt, UpdatedAt)
            VALUES (@VehicleId, @UserId, @LitresFilled, @PricePerLitre, @TotalCost, @OdometerReading, @FilledAt, @Notes, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();";
        return await conn.ExecuteScalarAsync<int>(sql, fuelLog);
    }

    public async Task<bool> UpdateAsync(FuelLog fuelLog)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            UPDATE FuelLogs
            SET LitresFilled    = @LitresFilled,
                PricePerLitre   = @PricePerLitre,
                TotalCost       = @TotalCost,
                OdometerReading = @OdometerReading,
                FilledAt        = @FilledAt,
                Notes           = @Notes,
                UpdatedAt       = @UpdatedAt
            WHERE Id = @Id AND UserId = @UserId";
        var affected = await conn.ExecuteAsync(sql, fuelLog);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        using var conn = _factory.CreateConnection();
        var affected = await conn.ExecuteAsync(
            "DELETE FROM FuelLogs WHERE Id = @Id AND UserId = @UserId",
            new { Id = id, UserId = userId });
        return affected > 0;
    }
}

using Dapper;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;
using VehicleExpense.Infrastructure.Data;

namespace VehicleExpense.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly DbConnectionFactory _factory;

    public VehicleRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<IEnumerable<Vehicle>> GetByUserIdAsync(int userId)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryAsync<Vehicle>(
            "SELECT * FROM Vehicles WHERE UserId = @UserId", new { UserId = userId });
    }

    public async Task<Vehicle?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<Vehicle>(
            "SELECT * FROM Vehicles WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(Vehicle vehicle)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            INSERT INTO Vehicles (UserId, Name, RegistrationNumber, VehicleType, FuelType, Year, Brand, Model, CreatedAt, UpdatedAt)
            VALUES (@UserId, @Name, @RegistrationNumber, @VehicleType, @FuelType, @Year, @Brand, @Model, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();";
        return await conn.ExecuteScalarAsync<int>(sql, vehicle);
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        using var conn = _factory.CreateConnection();
        var sql = @"
            UPDATE Vehicles
            SET Name = @Name, RegistrationNumber = @RegistrationNumber, VehicleType = @VehicleType,
                FuelType = @FuelType, Year = @Year, Brand = @Brand, Model = @Model, UpdatedAt = @UpdatedAt
            WHERE Id = @Id";
        await conn.ExecuteAsync(sql, vehicle);
    }

    public async Task DeleteAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        await conn.ExecuteAsync("DELETE FROM Vehicles WHERE Id = @Id", new { Id = id });
    }
}

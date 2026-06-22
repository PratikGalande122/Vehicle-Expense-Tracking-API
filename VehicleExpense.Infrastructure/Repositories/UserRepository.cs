using Dapper;
using Microsoft.Data.Sqlite;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;
using VehicleExpense.Infrastructure.Data;

namespace VehicleExpense.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DbConnectionFactory _factory;

    public UserRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Email = @Email AND IsDeleted = 0", new { Email = email });
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var conn = _factory.CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id AND IsDeleted = 0", new { Id = id });
    }

    public async Task<int> CreateAsync(User user)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
            INSERT INTO Users
                (FullName, Email, MobileNumber, ProfileImageUrl, DateOfBirth, Gender,
                 OtpHash, OtpExpiry, RefreshToken, RefreshTokenExpiry, CreatedAt, UpdatedAt, IsDeleted)
            VALUES
                (@FullName, @Email, @MobileNumber, @ProfileImageUrl, @DateOfBirth, @Gender,
                 @OtpHash, @OtpExpiry, @RefreshToken, @RefreshTokenExpiry, @CreatedAt, @UpdatedAt, @IsDeleted);
            SELECT last_insert_rowid();";
        return await conn.ExecuteScalarAsync<int>(sql, user);
    }

    public async Task UpdateAsync(User user)
    {
        using var conn = _factory.CreateConnection();
        const string sql = @"
            UPDATE Users
            SET FullName                    = @FullName,
                MobileNumber                = @MobileNumber,
                ProfileImageUrl             = @ProfileImageUrl,
                DateOfBirth                 = @DateOfBirth,
                Gender                      = @Gender,
                Address                     = @Address,
                City                        = @City,
                State                       = @State,
                Country                     = @Country,
                Pincode                     = @Pincode,
                BloodGroup                  = @BloodGroup,
                MedicalConditions           = @MedicalConditions,
                Allergies                   = @Allergies,
                EmergencyContactName        = @EmergencyContactName,
                EmergencyContactNumber      = @EmergencyContactNumber,
                EmergencyContactRelation    = @EmergencyContactRelation,
                InsuranceProvider           = @InsuranceProvider,
                InsurancePolicyNumber       = @InsurancePolicyNumber,
                PreferredMechanicName       = @PreferredMechanicName,
                PreferredMechanicContact    = @PreferredMechanicContact,
                PreferredHospital           = @PreferredHospital,
                PreferredHospitalContact    = @PreferredHospitalContact,
                DrivingLicenseNumber        = @DrivingLicenseNumber,
                LicenseExpiryDate           = @LicenseExpiryDate,
                RiderNotes                  = @RiderNotes,
                OtpHash                     = @OtpHash,
                OtpExpiry                   = @OtpExpiry,
                RefreshToken                = @RefreshToken,
                RefreshTokenExpiry          = @RefreshTokenExpiry,
                UpdatedAt                   = @UpdatedAt,
                IsDeleted                   = @IsDeleted
            WHERE Id = @Id";
        await conn.ExecuteAsync(sql, user);
    }

    /// <summary>
    /// Deletes the user and all related data inside a single SQLite transaction.
    /// Cascade order: Reminders ? FuelLogs ? Expenses ? Vehicles ? User.
    /// </summary>
    public async Task DeleteWithAllDataAsync(int userId)
    {
        using var conn = (SqliteConnection)_factory.CreateConnection();
        await conn.OpenAsync();

        using var tx = await conn.BeginTransactionAsync();
        try
        {
            // Enable FK enforcement inside this connection
            await conn.ExecuteAsync("PRAGMA foreign_keys=ON;", transaction: tx);

            // Cascade: delete child records for all vehicles owned by user
            await conn.ExecuteAsync(
                "DELETE FROM Reminders WHERE VehicleId IN (SELECT Id FROM Vehicles WHERE UserId = @UserId)",
                new { UserId = userId }, tx);

            await conn.ExecuteAsync(
                "DELETE FROM FuelLogs WHERE VehicleId IN (SELECT Id FROM Vehicles WHERE UserId = @UserId)",
                new { UserId = userId }, tx);

            await conn.ExecuteAsync(
                "DELETE FROM Expenses WHERE VehicleId IN (SELECT Id FROM Vehicles WHERE UserId = @UserId)",
                new { UserId = userId }, tx);

            await conn.ExecuteAsync(
                "DELETE FROM Vehicles WHERE UserId = @UserId",
                new { UserId = userId }, tx);

            // Invalidate sessions by clearing tokens before deleting
            await conn.ExecuteAsync(
                "UPDATE Users SET RefreshToken = NULL, RefreshTokenExpiry = NULL WHERE Id = @UserId",
                new { UserId = userId }, tx);

            await conn.ExecuteAsync(
                "DELETE FROM Users WHERE Id = @UserId",
                new { UserId = userId }, tx);

            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}


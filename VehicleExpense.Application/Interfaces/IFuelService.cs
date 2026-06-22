using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Business operations for fuel log management.
/// </summary>
public interface IFuelService
{
    Task<IEnumerable<FuelLog>> GetFuelLogsAsync(int userId);
    Task<FuelLog?> AddFuelLogAsync(int userId, AddFuelRequest request);
    Task<FuelLog?> UpdateFuelLogAsync(int userId, int id, UpdateFuelRequest request);
    /// <summary>Deletes a fuel log owned by the user. Returns false if not found or not owned.</summary>
    Task<bool> DeleteFuelLogAsync(int userId, int id);
}

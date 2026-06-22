using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

public interface IFuelRepository
{
    Task<IEnumerable<FuelLog>> GetByUserIdAsync(int userId);
    Task<FuelLog?> GetByIdAsync(int id, int userId);
    Task<int> CreateAsync(FuelLog fuelLog);
    Task<bool> UpdateAsync(FuelLog fuelLog);
    /// <summary>Deletes a fuel log only if it belongs to the given user. Returns true if deleted.</summary>
    Task<bool> DeleteAsync(int id, int userId);
}

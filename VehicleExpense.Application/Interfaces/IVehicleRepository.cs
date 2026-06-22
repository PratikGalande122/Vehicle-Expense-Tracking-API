using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle>> GetByUserIdAsync(int userId);
    Task<Vehicle?> GetByIdAsync(int id);
    Task<int> CreateAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
    Task DeleteAsync(int id);
}

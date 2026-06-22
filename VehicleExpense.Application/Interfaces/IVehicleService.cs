using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.DTOs.Responses;

namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Business operations for vehicle management.
/// </summary>
public interface IVehicleService
{
    Task<IEnumerable<VehicleResponse>> GetVehiclesAsync(int userId);
    Task<VehicleResponse?> AddVehicleAsync(int userId, AddVehicleRequest request);
    Task<VehicleResponse?> UpdateVehicleAsync(int userId, int vehicleId, AddVehicleRequest request);
    Task<bool> DeleteVehicleAsync(int userId, int vehicleId);
}

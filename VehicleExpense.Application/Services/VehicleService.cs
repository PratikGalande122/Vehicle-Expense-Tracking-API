using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.DTOs.Responses;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Services;

/// <summary>
/// Handles CRUD operations for vehicles belonging to a user.
/// </summary>
public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<VehicleResponse>> GetVehiclesAsync(int userId)
    {
        var vehicles = await _vehicleRepository.GetByUserIdAsync(userId);
        return vehicles.Select(MapToResponse);
    }

    public async Task<VehicleResponse?> AddVehicleAsync(int userId, AddVehicleRequest request)
    {
        var vehicle = new Vehicle
        {
            UserId = userId,
            Name = request.Name,
            RegistrationNumber = request.RegistrationNumber,
            VehicleType = request.VehicleType,
            FuelType = request.FuelType,
            Year = request.Year,
            Brand = request.Brand,
            Model = request.Model
        };

        var id = await _vehicleRepository.CreateAsync(vehicle);
        vehicle.Id = id;
        return MapToResponse(vehicle);
    }

    public async Task<VehicleResponse?> UpdateVehicleAsync(int userId, int vehicleId, AddVehicleRequest request)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle is null || vehicle.UserId != userId) return null;

        vehicle.Name = request.Name;
        vehicle.RegistrationNumber = request.RegistrationNumber;
        vehicle.VehicleType = request.VehicleType;
        vehicle.FuelType = request.FuelType;
        vehicle.Year = request.Year;
        vehicle.Brand = request.Brand;
        vehicle.Model = request.Model;
        vehicle.UpdatedAt = DateTime.UtcNow;

        await _vehicleRepository.UpdateAsync(vehicle);
        return MapToResponse(vehicle);
    }

    public async Task<bool> DeleteVehicleAsync(int userId, int vehicleId)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId);
        if (vehicle is null || vehicle.UserId != userId) return false;

        await _vehicleRepository.DeleteAsync(vehicleId);
        return true;
    }

    private static VehicleResponse MapToResponse(Vehicle v) =>
        new(v.Id, v.Name, v.RegistrationNumber, v.VehicleType, v.FuelType, v.Year, v.Brand, v.Model);
}

using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Request payload for adding a new vehicle.</summary>
public record AddVehicleRequest(
    string Name,
    string RegistrationNumber,
    VehicleType VehicleType,
    FuelType FuelType,
    int Year,
    string? Brand,
    string? Model
);

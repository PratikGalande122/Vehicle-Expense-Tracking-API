using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Application.DTOs.Responses;

/// <summary>Vehicle data returned to the client.</summary>
public record VehicleResponse(
    int Id,
    string Name,
    string RegistrationNumber,
    VehicleType VehicleType,
    FuelType FuelType,
    int Year,
    string? Brand,
    string? Model
);

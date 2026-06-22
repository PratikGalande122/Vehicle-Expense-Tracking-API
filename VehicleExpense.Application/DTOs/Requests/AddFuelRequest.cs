namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Request payload for logging a fuel fill-up.</summary>
public record AddFuelRequest(
    int VehicleId,
    decimal LitresFilled,
    decimal PricePerLitre,
    decimal OdometerReading,
    DateTime FilledAt,
    string? Notes
);

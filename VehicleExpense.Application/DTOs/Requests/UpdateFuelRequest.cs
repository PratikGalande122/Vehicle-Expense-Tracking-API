namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Request payload for editing an existing fuel log entry.</summary>
public record UpdateFuelRequest(
    decimal LitresFilled,
    decimal PricePerLitre,
    decimal OdometerReading,
    DateTime FilledAt,
    string? Notes
);

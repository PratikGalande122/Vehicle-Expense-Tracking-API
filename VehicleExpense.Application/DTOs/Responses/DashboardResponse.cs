namespace VehicleExpense.Application.DTOs.Responses;

/// <summary>Aggregated summary shown on the user's dashboard.</summary>
public record DashboardResponse(
    int TotalVehicles,
    decimal TotalFuelCost,
    decimal TotalExpenseCost,
    decimal TotalSpent,
    IEnumerable<VehicleExpenseSummary> VehicleSummaries
);

public record VehicleExpenseSummary(
    int VehicleId,
    string VehicleName,
    decimal FuelCost,
    decimal OtherExpenses,
    decimal Total
);

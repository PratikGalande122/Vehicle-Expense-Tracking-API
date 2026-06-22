using VehicleExpense.Application.DTOs.Responses;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Application.Services;

/// <summary>
/// Aggregates vehicle, fuel, and expense data for the user's dashboard.
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IFuelRepository _fuelRepository;
    private readonly IExpenseRepository _expenseRepository;

    public DashboardService(
        IVehicleRepository vehicleRepository,
        IFuelRepository fuelRepository,
        IExpenseRepository expenseRepository)
    {
        _vehicleRepository = vehicleRepository;
        _fuelRepository = fuelRepository;
        _expenseRepository = expenseRepository;
    }

    public async Task<DashboardResponse> GetDashboardAsync(int userId)
    {
        var vehicles = (await _vehicleRepository.GetByUserIdAsync(userId)).ToList();
        var fuelLogs = (await _fuelRepository.GetByUserIdAsync(userId)).ToList();
        var expenses = (await _expenseRepository.GetByUserIdAsync(userId)).ToList();

        var totalFuelCost = fuelLogs.Sum(f => f.TotalCost);
        var totalExpenseCost = expenses.Sum(e => e.Amount);

        var summaries = vehicles.Select(v => new VehicleExpenseSummary(
            v.Id,
            v.Name,
            fuelLogs.Where(f => f.VehicleId == v.Id).Sum(f => f.TotalCost),
            expenses.Where(e => e.VehicleId == v.Id).Sum(e => e.Amount),
            fuelLogs.Where(f => f.VehicleId == v.Id).Sum(f => f.TotalCost) +
            expenses.Where(e => e.VehicleId == v.Id).Sum(e => e.Amount)
        ));

        return new DashboardResponse(
            vehicles.Count,
            totalFuelCost,
            totalExpenseCost,
            totalFuelCost + totalExpenseCost,
            summaries
        );
    }
}

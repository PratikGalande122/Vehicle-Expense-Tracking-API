using VehicleExpense.Application.DTOs.Responses;

namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Aggregated dashboard data for a user.
/// </summary>
public interface IDashboardService
{
    Task<DashboardResponse> GetDashboardAsync(int userId);
}

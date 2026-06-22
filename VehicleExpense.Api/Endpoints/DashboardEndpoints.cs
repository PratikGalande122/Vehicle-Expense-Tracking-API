using System.Security.Claims;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Api.Endpoints;

/// <summary>
/// Dashboard endpoint that returns aggregated totals for the authenticated user.
/// </summary>
public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dashboard", async (ClaimsPrincipal user, IDashboardService dashboardService) =>
        {
            var userId = int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var dashboard = await dashboardService.GetDashboardAsync(userId);
            return Results.Ok(dashboard);
        })
        .WithTags("Dashboard")
        .RequireAuthorization()
        .WithName("GetDashboard")
        .WithSummary("Returns expense summary and totals for the authenticated user.");
    }
}

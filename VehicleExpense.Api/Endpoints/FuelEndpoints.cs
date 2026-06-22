using System.Security.Claims;
using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Api.Endpoints;

/// <summary>
/// Fuel log endpoints. All routes require JWT authentication.
/// </summary>
public static class FuelEndpoints
{
    public static void MapFuelEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/fuel")
            .WithTags("Fuel")
            .RequireAuthorization();

        // GET /api/fuel
        group.MapGet("/", async (ClaimsPrincipal user, IFuelService fuelService) =>
        {
            var userId = GetUserId(user);
            var logs = await fuelService.GetFuelLogsAsync(userId);
            return Results.Ok(logs);
        })
        .WithName("GetFuelLogs")
        .WithSummary("Returns all fuel logs for the authenticated user.");

        // POST /api/fuel
        group.MapPost("/", async (AddFuelRequest request, ClaimsPrincipal user, IFuelService fuelService) =>
        {
            var userId = GetUserId(user);
            var log = await fuelService.AddFuelLogAsync(userId, request);
            return Results.Created($"/api/fuel/{log!.Id}", log);
        })
        .WithName("AddFuelLog")
        .WithSummary("Adds a new fuel log entry.");

        // PUT /api/fuel/{id}
        group.MapPut("/{id:int}", async (int id, UpdateFuelRequest request, ClaimsPrincipal user, IFuelService fuelService) =>
        {
            var userId = GetUserId(user);
            var updated = await fuelService.UpdateFuelLogAsync(userId, id, request);
            return updated is not null
                ? Results.Ok(updated)
                : Results.NotFound(new { message = "Fuel log not found or not owned by user." });
        })
        .WithName("UpdateFuelLog")
        .WithSummary("Updates an existing fuel log entry by ID.");

        // DELETE /api/fuel/{id}
        group.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IFuelService fuelService) =>
        {
            var userId = GetUserId(user);
            var deleted = await fuelService.DeleteFuelLogAsync(userId, id);
            return deleted ? Results.NoContent() : Results.NotFound(new { message = "Fuel log not found or not owned by user." });
        })
        .WithName("DeleteFuelLog")
        .WithSummary("Deletes a fuel log entry by ID.");
    }

    private static int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

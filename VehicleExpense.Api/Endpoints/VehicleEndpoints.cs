using System.Security.Claims;
using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Api.Endpoints;

/// <summary>
/// Vehicle CRUD endpoints. All routes require JWT authentication.
/// </summary>
public static class VehicleEndpoints
{
    public static void MapVehicleEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/vehicles")
            .WithTags("Vehicles")
            .RequireAuthorization();

        // GET /api/vehicles
        group.MapGet("/", async (ClaimsPrincipal user, IVehicleService vehicleService) =>
        {
            var userId = GetUserId(user);
            var vehicles = await vehicleService.GetVehiclesAsync(userId);
            return Results.Ok(vehicles);
        })
        .WithName("GetVehicles")
        .WithSummary("Returns all vehicles for the authenticated user.");

        // POST /api/vehicles
        group.MapPost("/", async (AddVehicleRequest request, ClaimsPrincipal user, IVehicleService vehicleService) =>
        {
            var userId = GetUserId(user);
            var vehicle = await vehicleService.AddVehicleAsync(userId, request);
            return Results.Created($"/api/vehicles/{vehicle!.Id}", vehicle);
        })
        .WithName("AddVehicle")
        .WithSummary("Adds a new vehicle for the authenticated user.");

        // PUT /api/vehicles/{id}
        group.MapPut("/{id:int}", async (int id, AddVehicleRequest request, ClaimsPrincipal user, IVehicleService vehicleService) =>
        {
            var userId = GetUserId(user);
            var vehicle = await vehicleService.UpdateVehicleAsync(userId, id, request);
            return vehicle is not null ? Results.Ok(vehicle) : Results.NotFound();
        })
        .WithName("UpdateVehicle")
        .WithSummary("Updates an existing vehicle.");

        // DELETE /api/vehicles/{id}
        group.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IVehicleService vehicleService) =>
        {
            var userId = GetUserId(user);
            var deleted = await vehicleService.DeleteVehicleAsync(userId, id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteVehicle")
        .WithSummary("Deletes a vehicle.");
    }

    private static int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

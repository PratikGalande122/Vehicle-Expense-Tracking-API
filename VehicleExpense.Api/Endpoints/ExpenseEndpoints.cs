using System.Security.Claims;
using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Api.Endpoints;

/// <summary>
/// Expense endpoints. All routes require JWT authentication.
/// </summary>
public static class ExpenseEndpoints
{
    public static void MapExpenseEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/expenses")
            .WithTags("Expenses")
            .RequireAuthorization();

        // GET /api/expenses
        group.MapGet("/", async (ClaimsPrincipal user, IExpenseService expenseService) =>
        {
            var userId = GetUserId(user);
            var expenses = await expenseService.GetExpensesAsync(userId);
            return Results.Ok(expenses);
        })
        .WithName("GetExpenses")
        .WithSummary("Returns all expenses for the authenticated user.");

        // POST /api/expenses
        group.MapPost("/", async (AddExpenseRequest request, ClaimsPrincipal user, IExpenseService expenseService) =>
        {
            var userId = GetUserId(user);
            var expense = await expenseService.AddExpenseAsync(userId, request);
            return Results.Created($"/api/expenses/{expense!.Id}", expense);
        })
        .WithName("AddExpense")
        .WithSummary("Adds a new expense entry.");

        // PUT /api/expenses/{id}
        group.MapPut("/{id:int}", async (int id, UpdateExpenseRequest request, ClaimsPrincipal user, IExpenseService expenseService) =>
        {
            var userId = GetUserId(user);
            var updated = await expenseService.UpdateExpenseAsync(userId, id, request);
            return updated is not null
                ? Results.Ok(updated)
                : Results.NotFound(new { message = "Expense not found or not owned by user." });
        })
        .WithName("UpdateExpense")
        .WithSummary("Updates an existing expense entry by ID.");

        // DELETE /api/expenses/{id}
        group.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, IExpenseService expenseService) =>
        {
            var userId = GetUserId(user);
            var deleted = await expenseService.DeleteExpenseAsync(userId, id);
            return deleted ? Results.NoContent() : Results.NotFound(new { message = "Expense not found or not owned by user." });
        })
        .WithName("DeleteExpense")
        .WithSummary("Deletes an expense entry by ID.");
    }

    private static int GetUserId(ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

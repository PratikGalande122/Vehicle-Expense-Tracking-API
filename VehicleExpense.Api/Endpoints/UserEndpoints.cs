using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Api.Endpoints;

/// <summary>
/// User profile and account management endpoints. All routes require JWT authentication.
/// </summary>
public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/user")
            .WithTags("User")
            .RequireAuthorization();

        // GET /api/user/profile
        group.MapGet("/profile", async (ClaimsPrincipal principal, IUserService userService) =>
        {
            var userId = GetUserId(principal);
            if (userId is null)
                return Results.Unauthorized();

            var profile = await userService.GetProfileAsync(userId.Value);
            return profile is not null
                ? Results.Ok(profile)
                : Results.NotFound(new { message = "User profile not found." });
        })
        .WithName("GetUserProfile")
        .WithSummary("Returns the authenticated user's profile.");

        // PUT /api/user/profile
        group.MapPut("/profile", async (
            ClaimsPrincipal principal,
            [FromBody] UpdateUserProfileRequest request,
            IUserService userService) =>
        {
            var userId = GetUserId(principal);
            if (userId is null)
                return Results.Unauthorized();

            // Validate data annotations (required fields, regex, max length)
            var validationErrors = ValidateRequest(request);
            if (validationErrors.Count > 0)
                return Results.BadRequest(new { message = "Validation failed.", errors = validationErrors });

            // Business rule: DateOfBirth must be in the past and user >= 16 years old
            if (request.DateOfBirth >= DateTime.UtcNow.Date)
                return Results.BadRequest(new { message = "DateOfBirth must be a past date." });

            if ((DateTime.UtcNow - request.DateOfBirth).TotalDays < 16 * 365)
                return Results.BadRequest(new { message = "User must be at least 16 years old." });

            var updated = await userService.UpdateProfileAsync(userId.Value, request);
            return updated is not null
                ? Results.Ok(updated)
                : Results.NotFound(new { message = "User not found." });
        })
        .WithName("UpdateUserProfile")
        .WithSummary("Updates the authenticated user's profile. MobileNumber, DateOfBirth, BloodGroup, and emergency contact fields are required.");

        // POST /api/user/profile-image
        group.MapPost("/profile-image", async (
            ClaimsPrincipal principal,
            [FromBody] UploadProfileImageRequest request,
            IUserService userService) =>
        {
            var userId = GetUserId(principal);
            if (userId is null)
                return Results.Unauthorized();

            if (string.IsNullOrWhiteSpace(request.ProfileImageUrl))
                return Results.BadRequest(new { message = "ProfileImageUrl is required." });

            var updated = await userService.UpdateProfileImageAsync(userId.Value, request);
            return updated is not null
                ? Results.Ok(updated)
                : Results.NotFound(new { message = "User not found." });
        })
        .WithName("UploadProfileImage")
        .WithSummary("Sets or replaces the authenticated user's profile image URL.");

        // DELETE /api/user/account
        group.MapDelete("/account", async (ClaimsPrincipal principal, IUserService userService) =>
        {
            var userId = GetUserId(principal);
            if (userId is null)
                return Results.Unauthorized();

            var deleted = await userService.DeleteAccountAsync(userId.Value);
            return deleted
                ? Results.Ok(new { message = "Account and all associated data deleted successfully." })
                : Results.NotFound(new { message = "User not found or already deleted." });
        })
        .WithName("DeleteAccount")
        .WithSummary("Permanently deletes the authenticated user's account and all associated data.");
    }

    private static int? GetUserId(ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var id) ? id : null;
    }

    private static List<string> ValidateRequest(object request)
    {
        var context = new ValidationContext(request);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, context, results, validateAllProperties: true);
        return results.Select(r => r.ErrorMessage ?? "Invalid field.").ToList();
    }
}

using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.DTOs.Responses;

namespace VehicleExpense.Application.Interfaces;

/// <summary>User profile and account management operations.</summary>
public interface IUserService
{
    /// <summary>Returns the profile of the authenticated user.</summary>
    Task<UserProfileResponse?> GetProfileAsync(int userId);

    /// <summary>Updates editable profile fields for the authenticated user.</summary>
    Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateUserProfileRequest request);

    /// <summary>Sets or replaces the profile image URL for the authenticated user.</summary>
    Task<UserProfileResponse?> UpdateProfileImageAsync(int userId, UploadProfileImageRequest request);

    /// <summary>
    /// Permanently deletes the user and all associated data in a single transaction.
    /// Cascade order: Reminders ? FuelLogs ? Expenses ? Vehicles ? User.
    /// </summary>
    Task<bool> DeleteAccountAsync(int userId);
}

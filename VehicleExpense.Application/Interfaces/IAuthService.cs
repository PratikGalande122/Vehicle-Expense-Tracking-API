using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.DTOs.Responses;

namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Handles OTP-based authentication flow.
/// </summary>
public interface IAuthService
{
    /// <returns>The raw OTP string (for dev/test use) or null if sent via email successfully.</returns>
    Task<string?> SendOtpAsync(SendOtpRequest request);
    Task<AuthResponse?> VerifyOtpAsync(VerifyOtpRequest request);
}

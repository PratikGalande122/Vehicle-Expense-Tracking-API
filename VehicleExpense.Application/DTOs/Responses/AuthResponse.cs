namespace VehicleExpense.Application.DTOs.Responses;

/// <summary>Returned after successful OTP verification.</summary>
public record AuthResponse(
    int UserId,
    string Name,
    string Email,
    string AccessToken,
    string RefreshToken
);

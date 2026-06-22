namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Verifies the OTP sent to the user's email.</summary>
public record VerifyOtpRequest(string Email, string Otp);

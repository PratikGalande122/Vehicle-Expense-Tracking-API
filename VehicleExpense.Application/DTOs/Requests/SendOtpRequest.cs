namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Sends an OTP to the user's email address.</summary>
public record SendOtpRequest(string Email, string FullName);

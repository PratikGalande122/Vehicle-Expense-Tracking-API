namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Sends emails (e.g., OTP codes) to users.
/// </summary>
public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
}

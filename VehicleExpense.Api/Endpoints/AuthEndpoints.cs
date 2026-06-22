using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Api.Endpoints;

/// <summary>
/// Auth endpoints: send OTP and verify OTP to obtain a JWT.
/// </summary>
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // POST /api/auth/send-otp
        group.MapPost("/send-otp", async (SendOtpRequest request, IAuthService authService) =>
        {
            var rawOtp = await authService.SendOtpAsync(request);
            return Results.Ok(new { message = "OTP generated successfully.", otp = rawOtp });
        })
        .WithName("SendOtp")
        .WithSummary("Sends a one-time password to the provided email address.");

        // POST /api/auth/verify-otp
        group.MapPost("/verify-otp", async (VerifyOtpRequest request, IAuthService authService) =>
        {
            var response = await authService.VerifyOtpAsync(request);
            return response is not null
                ? Results.Ok(response)
                : Results.Unauthorized();
        })
        .WithName("VerifyOtp")
        .WithSummary("Verifies the OTP and returns a JWT access token.");
    }
}

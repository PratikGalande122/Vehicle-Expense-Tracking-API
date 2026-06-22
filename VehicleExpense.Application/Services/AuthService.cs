using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.DTOs.Responses;
using VehicleExpense.Application.Interfaces;
using VehicleExpense.Domain.Constants;
using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Services;

/// <summary>
/// Handles OTP generation, email dispatch, and JWT issuance.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IEmailService emailService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<string?> SendOtpAsync(SendOtpRequest request)
    {
        // Generate a 6-digit OTP
        var otp = new Random().Next(100000, 999999).ToString();
        var otpHash = BCrypt.Net.BCrypt.HashPassword(otp);
        var expiry = DateTime.UtcNow.AddMinutes(AppConstants.OtpExpiryMinutes);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                OtpHash = otpHash,
                OtpExpiry = expiry
            };
            await _userRepository.CreateAsync(user);
        }
        else
        {
            user.FullName = request.FullName;
            user.OtpHash = otpHash;
            user.OtpExpiry = expiry;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        // SMTP skipped — return OTP directly for testing
        return otp;
    }

    public async Task<AuthResponse?> VerifyOtpAsync(VerifyOtpRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null || user.OtpHash is null || user.OtpExpiry < DateTime.UtcNow)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Otp, user.OtpHash))
            return null;

        // Clear OTP after successful verification
        user.OtpHash = null;
        user.OtpExpiry = null;

        // Generate refresh token (placeholder support)
        var refreshToken = _jwtService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(AppConstants.RefreshTokenExpiryDays);
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var accessToken = _jwtService.GenerateToken(user);

        return new AuthResponse(user.Id, user.FullName, user.Email, accessToken, refreshToken);
    }
}

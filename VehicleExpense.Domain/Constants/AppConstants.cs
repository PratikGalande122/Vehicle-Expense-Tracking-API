namespace VehicleExpense.Domain.Constants;

/// <summary>
/// Application-wide constants.
/// </summary>
public static class AppConstants
{
    public const string JwtSectionKey = "Jwt";
    public const string DefaultRole = "User";
    public const int OtpExpiryMinutes = 5;
    public const int JwtExpiryMinutes = 60;
    public const int RefreshTokenExpiryDays = 7;
}

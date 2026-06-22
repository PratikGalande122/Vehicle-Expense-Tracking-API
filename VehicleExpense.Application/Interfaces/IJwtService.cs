using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

/// <summary>
/// Generates JWT access tokens and handles refresh token logic.
/// </summary>
public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}

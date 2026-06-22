using VehicleExpense.Domain.Entities;

namespace VehicleExpense.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);
    Task<int> CreateAsync(User user);
    Task UpdateAsync(User user);

    /// <summary>Deletes the user and all related data inside a single transaction.</summary>
    Task DeleteWithAllDataAsync(int userId);
}

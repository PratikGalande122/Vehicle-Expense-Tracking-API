using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using VehicleExpense.Application.DTOs.Requests;
using VehicleExpense.Application.DTOs.Responses;
using VehicleExpense.Application.Interfaces;

namespace VehicleExpense.Application.Services;

/// <summary>
/// Handles user profile view, update, image upload, and account deletion.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<UserProfileResponse?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.IsDeleted)
            return null;

        return MapToResponse(user);
    }

    public async Task<UserProfileResponse?> UpdateProfileAsync(int userId, UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.IsDeleted)
            return null;

        // Optional identity
        if (request.FullName is not null) user.FullName = request.FullName;
        if (request.ProfileImageUrl is not null) user.ProfileImageUrl = request.ProfileImageUrl;
        if (request.Gender is not null) user.Gender = request.Gender;

        // Mandatory safety fields
        user.MobileNumber = request.MobileNumber;
        user.DateOfBirth = request.DateOfBirth;
        user.BloodGroup = request.BloodGroup;
        user.EmergencyContactName = request.EmergencyContactName;
        user.EmergencyContactNumber = request.EmergencyContactNumber;
        user.EmergencyContactRelation = request.EmergencyContactRelation;

        // Optional address
        user.Address = request.Address;
        user.City = request.City;
        user.State = request.State;
        user.Country = request.Country;
        user.Pincode = request.Pincode;

        // Optional medical
        user.MedicalConditions = request.MedicalConditions;
        user.Allergies = request.Allergies;

        // Optional insurance
        user.InsuranceProvider = request.InsuranceProvider;
        user.InsurancePolicyNumber = request.InsurancePolicyNumber;

        // Optional preferred services
        user.PreferredMechanicName = request.PreferredMechanicName;
        user.PreferredMechanicContact = request.PreferredMechanicContact;
        user.PreferredHospital = request.PreferredHospital;
        user.PreferredHospitalContact = request.PreferredHospitalContact;

        // Optional license
        user.DrivingLicenseNumber = request.DrivingLicenseNumber;
        user.LicenseExpiryDate = request.LicenseExpiryDate;

        // Optional notes
        user.RiderNotes = request.RiderNotes;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("User {UserId} updated their profile.", userId);
        return MapToResponse(user);
    }

    public async Task<UserProfileResponse?> UpdateProfileImageAsync(int userId, UploadProfileImageRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.IsDeleted)
            return null;

        user.ProfileImageUrl = request.ProfileImageUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);

        _logger.LogInformation("User {UserId} updated their profile image.", userId);
        return MapToResponse(user);
    }

    public async Task<bool> DeleteAccountAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.IsDeleted)
            return false;

        await _userRepository.DeleteWithAllDataAsync(userId);

        _logger.LogWarning("User {UserId} permanently deleted their account and all associated data.", userId);
        return true;
    }

    private static UserProfileResponse MapToResponse(Domain.Entities.User u) =>
        new(u.Id, u.FullName, u.Email, u.MobileNumber, u.ProfileImageUrl,
            u.DateOfBirth, u.Gender,
            u.Address, u.City, u.State, u.Country, u.Pincode,
            u.BloodGroup, u.MedicalConditions, u.Allergies,
            u.EmergencyContactName, u.EmergencyContactNumber, u.EmergencyContactRelation,
            u.InsuranceProvider, u.InsurancePolicyNumber,
            u.PreferredMechanicName, u.PreferredMechanicContact,
            u.PreferredHospital, u.PreferredHospitalContact,
            u.DrivingLicenseNumber, u.LicenseExpiryDate,
            u.RiderNotes,
            u.CreatedAt, u.UpdatedAt);
}

using VehicleExpense.Domain.Enums;

namespace VehicleExpense.Application.DTOs.Responses;

/// <summary>Full user profile returned by GET and PUT /api/user/profile.</summary>
public record UserProfileResponse(
    int Id,
    string FullName,
    string Email,
    string? MobileNumber,
    string? ProfileImageUrl,
    DateTime? DateOfBirth,
    Gender? Gender,
    // Address
    string? Address,
    string? City,
    string? State,
    string? Country,
    string? Pincode,
    // Safety & medical
    BloodGroup? BloodGroup,
    string? MedicalConditions,
    string? Allergies,
    // Emergency contact
    string? EmergencyContactName,
    string? EmergencyContactNumber,
    string? EmergencyContactRelation,
    // Insurance
    string? InsuranceProvider,
    string? InsurancePolicyNumber,
    // Preferred services
    string? PreferredMechanicName,
    string? PreferredMechanicContact,
    string? PreferredHospital,
    string? PreferredHospitalContact,
    // License
    string? DrivingLicenseNumber,
    DateTime? LicenseExpiryDate,
    // Notes
    string? RiderNotes,
    // Audit
    DateTime CreatedAt,
    DateTime? UpdatedAt
);


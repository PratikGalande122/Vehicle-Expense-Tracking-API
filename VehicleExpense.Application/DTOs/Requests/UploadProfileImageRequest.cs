using System.ComponentModel.DataAnnotations;

namespace VehicleExpense.Application.DTOs.Requests;

/// <summary>Request payload for uploading or updating the user's profile image URL.</summary>
public record UploadProfileImageRequest(
    [Required, Url, MaxLength(2048)] string ProfileImageUrl
);

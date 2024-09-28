using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UserDto(
    
    int Id,

    [Required]                             // Username is required
    [StringLength(100, MinimumLength = 3)]  // Username must be between 3 and 100 characters
    string Username,

    [Required]                             // Email is required
    [EmailAddress]                         // Ensures a valid email format
    string Email,

    [Required]                             // FirstName is required
    [StringLength(50)]                     // FirstName max length is 50
    string FirstName,

    [StringLength(50)]                     // MiddleName max length is 50 (optional)
    string? MiddleName,                    // Nullable since not all users may have a middle name

    [Required]                             // LastName is required
    [StringLength(50)]                     // LastName max length is 50
    string LastName,

    [Required]                             // Password is required
    [StringLength(100, MinimumLength = 8)]  // Password must be between 8 and 100 characters
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$", 
        ErrorMessage = "Password must have at least 1 lowercase, 1 uppercase letter, and 1 digit.")]
    string Password
);

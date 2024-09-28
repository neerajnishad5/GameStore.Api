using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateUserDto(
    
    // [Required(ErrorMessage = "Username is required.")]
    [StringLength(100, ErrorMessage = "Username cannot be longer than 100 characters.")]
    string Username,

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters.")]
    string? FirstName,

    [StringLength(100, ErrorMessage = "Middle name cannot be longer than 100 characters.")]
    string MiddleName,

    // [Required(ErrorMessage = "Last name is required.")]
    [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters.")]
    string LastName,

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    string Email,

    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    string Password
);

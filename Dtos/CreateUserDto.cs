namespace GameStore.Api.Dtos;

public record class CreateUserDto
(
    string Username,
    string FirstName,
    string? MiddleName,
    string LastName, 
    string Email,
    string Password
);
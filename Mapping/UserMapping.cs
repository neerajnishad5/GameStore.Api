using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping;

public static class UserMapping
{

    public static User ToEntity(this CreateUserDto user)
    {
        return new User()
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            MiddleName = user.MiddleName!,
            Email = user.Email,
            Password = user.Password,
            Role = user.Role!
        };
    }

    public static User ToEntity(this UpdateUserDto user, int id)
    {
        return new User()
        {
            Id = id,
            Username = user.Username,
            FirstName = user.FirstName!,
            LastName = user.LastName,
            MiddleName = user.MiddleName!,
            Email = user.Email,
            Password = user.Password,
            Role = user.Role,
        };
    }

}

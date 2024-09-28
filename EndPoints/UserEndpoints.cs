using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Mapping;
using GameStore.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace GameStore.Api.EndPoints;

public static class UserEndpoints
{
  public static RouteGroupBuilder MapUsersEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("users");

    // CRUD Operations
    group.MapGet("/", async (GameStoreContext dbContext) =>
    {
      var users = await dbContext.Users.Select(user => user).AsNoTracking().ToListAsync();
      return Results.Ok(users); // Return 200 OK with the list of users
    }).WithTags("Users"); // Get all users

    // Get user by Id
    group.MapGet("/{id:int}", async (int id, GameStoreContext dbContext) =>
    {
      // Retrieve the user from the database
      var user = await dbContext.Users.FindAsync(id);

      // Check if the user was found
      if (user == null)
      {
        return Results.NotFound(); // Return 404 if user not found
      }

      // Return the user with a 200 OK response
      return Results.Ok(user);

    });

    group.MapPut("/{id:int}", async (int id, UpdateUserDto updatedUser, GameStoreContext dbContext) =>
    {

      var existingUser = await dbContext.Users.FindAsync(id);
      if (existingUser is null)
      {
        return Results.NotFound($"No User found with id: {id}");
      }

      // Update fields only if they are provided
      dbContext.Entry(existingUser).CurrentValues.SetValues(updatedUser.ToEntity(id));



      // Saving changes back to the database
      await dbContext.SaveChangesAsync();

      return Results.NoContent();
    });


    group.MapDelete("/{id:int}", async (int id, GameStoreContext dbContext) =>
    {
      // Find the user in the database
      var user = await dbContext.Users.FindAsync(id);

      // Check if the user exists
      if (user == null)
      {
        return Results.NotFound(); // Return 404 if user not found
      }

      // Remove the user from the database
      dbContext.Users.Remove(user);
      await dbContext.SaveChangesAsync(); // Save changes to the database

      // Return 204 No Content indicating successful deletion
      return Results.NoContent();
    }).WithName("DeleteUserById");

    // Authentication
    group.MapPost("/login", async (LoginDto loginDto, GameStoreContext dbContext) =>
    {
      // Find the user by their email or username
      var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);

      if (user == null)
      {
        return Results.NotFound("User not found!"); // Return 401 if the user is not found
      }

      // Verify the provided password against the stored hashed password
      var passwordVerificationResult = UserService.VerifyHashedPassword(user.Password, loginDto.Password);
  
      if (passwordVerificationResult == PasswordVerificationResult.Failed)
      {
        return Results.Unauthorized(); // Return 401 if password verification failed
      }

      // If password verification succeeded, return a success response
      return Results.Ok(new { Message = "Login successful" });
    }).WithTags("Auth").AllowAnonymous(); // User login


    group.MapPost("/signup", async (CreateUserDto newUser, GameStoreContext dbContext) =>
    {
      // Check for existing user with the same username or email using FirstOrDefaultAsync
      var existingUser = await dbContext.Users
        .AsNoTracking()
        .FirstOrDefaultAsync(user => user.Username == newUser.Username || user.Email == newUser.Email);

      if (existingUser != null)
      {
        return Results.Conflict("A user with the same username or email already exists!");
      }

      // hashing pass to store in db
      var hashedPassword = UserService.HashPassword(newUser.Password);


      // Convert DTO to User entity and add to database
      var user = newUser.ToEntity();

      // storing hashed password in converted DTO user entity
      user.Password = hashedPassword;


      await dbContext.Users.AddAsync(user);
      await dbContext.SaveChangesAsync();

      // Use Results.Created without route reference
      return Results.Created($"/users/{user.Id}", user);
    })
      .AllowAnonymous();

    // Authorization Testing Route
    group.MapGet("/profile", async (int id, GameStoreContext dbContext) =>
    {
      // Retrieve the user from the database
      var user = await dbContext.Users.FindAsync(id);

      // Check if the user was found
      if (user == null)
      {
        return Results.NotFound(); // Return 404 if user not found
      }

      // Return the user with a 200 OK response
      return Results.Ok(user);
    }); // Get profile of logged-in user

    return group;
  }
}
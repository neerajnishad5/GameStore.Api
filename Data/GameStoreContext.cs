using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public class GameStoreContext(DbContextOptions < GameStoreContext > options): DbContext(options) {
  // objects that need to be mapped to DB

  // DbSet is an obj that can be used to read & save instances of below types to DB

  public DbSet < Game > Games {get; set;} // here Set<Game>(); is creating our DB set instance 
  public DbSet < Genre > Genres {get; set;}
  public DbSet<User> Users { get; set; }

  // method will be executed as soon as we execute the migration
  // here we're populating the Genres as it's static data & not changed frequently

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity < Genre > ().HasData(
      new {
        Id = 1, Name = "Action"
      },
      new {
        Id = 2, Name = "Adventure"
      },
      new {
        Id = 3, Name = "RPG"
      },
      new {
        Id = 4, Name = "Strategy"
      },
      new {
        Id = 5, Name = "Sports"
      }
    );
    modelBuilder.Entity < User > ().HasData(
      new {
        Id = 1,
          Username = "john_doe",
          FirstName = "John",
          MiddleName = "A.",
          LastName = "Doe",
          Email = "john.doe@example.com",
          Password = "SecurePassword123",
          Role = "admin"
      });

  }
}
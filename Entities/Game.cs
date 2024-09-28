using System;

namespace GameStore.Api.Entities;

public class Game
{
    public int Id { get; set; }

    public required string Name { get; set; } // we've made this required else the compiler will ask to make this nullable as it's a non null property

    public int GenreId { get; set; }

    // we create this type of property for every foreign key, notice the type & name of property; use the same for other foreign keys that we'll make
    public Genre? Genre { get; set; }  // here, the question suggests that this property is nullable; so we can provide sometimes else don'ts

    public decimal Price { get; set; }

    public DateOnly ReleaseDate { get; set; }
}

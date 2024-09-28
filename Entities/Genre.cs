using System;

namespace GameStore.Api.Entities;

public class Genre
{
    public int Id { get; set; }

    public required string Name { get; set; } // we've made this required else the compiler will ask to make this nullable as it's a non null property
}

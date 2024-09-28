using GameStore.Api.Data;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GenreEndpoints
{
    public static RouteGroupBuilder MapGenresEndpoints(this WebApplication app)
    {

        // labels this genres for entering /genres controller
        var group = app.MapGroup("genres");

        group.MapGet("/", async(GameStoreContext dbContext) =>
        {
             // Check if there are any Genres before fetching the full data
            if (!await dbContext.Genres.AnyAsync())
            {
                return Results.NotFound(); // Return 404 if no games are found
            }

            // Fetch the genres & adding
            var games = await dbContext.Genres 
                                       .Select(genre => genre.ToDto())
                                       .AsNoTracking()                    // Improve performance for read-only data
                                       .ToListAsync();                    // Fetch data asynchronously

            return Results.Ok(games);
        });

        return group;
    }

}

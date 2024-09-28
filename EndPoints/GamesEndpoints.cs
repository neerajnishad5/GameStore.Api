using System;
using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.EndPoints;

public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";
    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("games")
                       .WithParameterValidation();

        // GET /games: get the list of games
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            // Check if there are any games before fetching the full data
            if (!await dbContext.Games.AnyAsync())
            {
                return Results.NotFound(); // Return 404 if no games are found
            }

            // Fetch the games including Genre, and map to GameSummaryDto
            var games = await dbContext.Games
                .Include(game => game.Genre)        // Include related Genre table
                .Select(game => game.ToGameSummaryDto())  // Project to DTO
                .AsNoTracking()                    // Improve performance for read-only data
                .ToListAsync();                    // Fetch data asynchronously

            return Results.Ok(games); // Return 200 OK with the list of games
        });

        // GET /games/{id}: get the particular game with ID
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            Game? game = await dbContext.Games.FindAsync(id); // here also we changed Find(earlier) to FindAsync to get the data Asynchronously
            return game is null ? Results.NotFound() : Results.Ok(game);
        }).WithName(GetGameEndpointName);

        // POST /games: adding a game via POST request
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();
            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();
            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        });

        // PUT /games/{id} - update any of the games
        group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound($"No game found with id: {id}");
            }

            // updating the DB if the game exist
            dbContext.Entry(existingGame).CurrentValues.SetValues(updatedGame.ToEntity(id));


            // saving changes back to the database
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/{id}: delete a game with ID
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            // here, we use where fn to find game w/ id and executeDelete() method to delete the game.
            await dbContext.Games.Where(game => game.Id == id)
                           .ExecuteDeleteAsync();

            await dbContext.SaveChangesAsync();
            return Results.NoContent();
        });

        return group;
    }
}

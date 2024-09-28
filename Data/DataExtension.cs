using System;
using Microsoft.EntityFrameworkCore;
namespace GameStore.Api.Data;

// this class is for running any missed migrations on app startup

public static class DataExtensions
{
    // here, the return type is Task because when someone calls for this method they expect a Task to await on
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

         // here we execute any pending migrations so this should be async
        await dbContext.Database.MigrateAsync(); 
    }
}

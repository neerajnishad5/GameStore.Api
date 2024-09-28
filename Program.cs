using System.Text;
using GameStore.Api.Data;
using GameStore.Api.EndPoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
         ValidIssuer = "GameStore",
        ValidAudience = "GameStoreUsers",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret_2314dfn98rtsdfnvw8!!"))
    };
});

// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
});

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();


// getting the connection string from the appSettings.json file stored inside configuration object
var connectionString = builder.Configuration.GetConnectionString("GameStore");

// here we're passing the connection string to implement Dependency Injection to register GameStoreContext;
// we're telling builder to use AddSqlite method w/ our GameStoreContext
builder.Services.AddSqlite<GameStoreContext>(connectionString);

// Add logging services
builder.Logging.AddConsole();

var app = builder.Build();

// map Games endpoints
app.MapGamesEndpoints();

// map genres endpoints
app.MapGenresEndpoints();

// map user endpoints
app.MapUsersEndpoints();

// running migrations on startup app if any migrations is missing
await app.MigrateDbAsync();

// Optionally configure error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();
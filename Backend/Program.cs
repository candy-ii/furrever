using Backend.Data;
using Backend.Models;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

// Reads Backend/.env into environment variables. Copy .env.example to .env and
// fill in your own Postgres credentials; .env is never committed.
Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- Postgres -------------------------------------------------------------
var connectionString =
    $"Host={Env.GetString("POSTGRES_HOST", "localhost")};" +
    $"Port={Env.GetInt("POSTGRES_PORT", 5432)};" +
    $"Database={Env.GetString("POSTGRES_DB")};" +
    $"Username={Env.GetString("POSTGRES_USER")};" +
    $"Password={Env.GetString("POSTGRES_PASSWORD")}";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Apply any migrations that haven't run yet (creating the database first if
// needed), then drop in two sample rows so the frontend has something to show.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Pets.Any())
    {
        db.Pets.AddRange(
            new Pet { Name = "Biscuit", Species = "Dog" },
            new Pet { Name = "Miso", Species = "Cat" });
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();

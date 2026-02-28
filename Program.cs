using Microsoft.EntityFrameworkCore;
using PhotoBooth.Data;
using PhotoBooth.Services;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Connection string setup
// =======================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Render gebruikt DATABASE_URL
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Ondersteunt postgres:// en postgresql://
    var regex = new Regex(
        @"postgres(?:ql)?:\/\/(?<user>[^:]+):(?<pass>[^@]+)@(?<host>[^:]+):(?<port>\d+)\/(?<db>.+)");

    var match = regex.Match(databaseUrl);

    if (match.Success)
    {
        connectionString =
            $"Host={match.Groups["host"].Value};" +
            $"Port={match.Groups["port"].Value};" +
            $"Database={match.Groups["db"].Value};" +
            $"Username={match.Groups["user"].Value};" +
            $"Password={match.Groups["pass"].Value};" +
            $"SSL Mode=Require;Trust Server Certificate=true";
    }
}

// =======================
// Services
// =======================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

builder.Services.AddScoped<IPhotoValidator, MockPhotoValidator>();

var app = builder.Build();

// =======================
// Middleware
// =======================

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// =======================
// Auto migrate in production
// =======================

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
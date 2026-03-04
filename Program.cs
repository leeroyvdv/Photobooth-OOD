using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Interfaces;
using PhotoBooth.Application.Services;
using PhotoBooth.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Connection string ophalen uit de configuratie (bijv. appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Database context registreren (PostgreSQL via Entity Framework)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Razor pages toevoegen (voor de web interface)
builder.Services.AddRazorPages();

// Blazor Server toevoegen voor de frontend
builder.Services.AddServerSideBlazor();

// Controllers toevoegen voor de API endpoints
builder.Services.AddControllers();

// Services registreren zodat ze gebruikt kunnen worden via dependency injection
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<PhotoValidationService>();

// Applicatie bouwen
var app = builder.Build();

// Static files gebruiken (css, js, images)
app.UseStaticFiles();

// Routing inschakelen
app.UseRouting();

// API controllers activeren
app.MapControllers();

// Blazor communicatie endpoint
app.MapBlazorHub();

// Fallback pagina voor Blazor
app.MapFallbackToPage("/_Host");

// Database migraties automatisch uitvoeren bij het starten
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Applicatie starten
app.Run();
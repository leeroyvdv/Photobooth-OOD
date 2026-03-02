using Microsoft.EntityFrameworkCore;
using PhotoBooth.Application.Interfaces;
using PhotoBooth.Application.Services;
using PhotoBooth.Infrastructure.Data;
using PhotoBooth.Services;

var builder = WebApplication.CreateBuilder(args);

// Alleen standaard connection string gebruiken
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<PhotoValidationService>();

builder.Services.AddScoped<PhotoBooth.Application.Interfaces.IPhotoRepository, MockPhotoValidator>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Auto migrate
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
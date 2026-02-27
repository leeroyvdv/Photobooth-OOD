using PhotoBooth.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// ✅ DIT TOEVOEGEN
builder.Services.AddControllers();

builder.Services.AddScoped<IPhotoValidator, MockPhotoValidator>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// ✅ DIT TOEVOEGEN
app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

using SuperLeague.Interfaces;
using SuperLeague.Repositories;

var builder = WebApplication.CreateBuilder(args);


//Registracija kontrolera
builder.Services.AddControllers();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();


var app = builder.Build();
app.UseHttpsRedirection();

// Dodaj middleware za serviranje statičkih fajlova
app.UseDefaultFiles(); // traži index.html
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

app.Run();

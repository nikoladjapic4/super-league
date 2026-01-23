using SuperLeague.Interfaces;
using SuperLeague.Repositories;
using SuperLeague.Services;

var builder = WebApplication.CreateBuilder(args);

// Registracija kontrolera
builder.Services.AddControllers();


//  Register Repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();

// Register Services
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

var app = builder.Build();

app.UseHttpsRedirection();

// Dodaj middleware za serviranje statičkih fajlova
app.UseDefaultFiles(); // traži index.html
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Run();
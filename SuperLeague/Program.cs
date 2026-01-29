using SuperLeague.Interfaces;
using SuperLeague.Repositories;
using SuperLeague.Services;

var builder = WebApplication.CreateBuilder(args);

// Registracija kontrolera
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Register Repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();

// Register Services
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();

var app = builder.Build();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Dodaj middleware za serviranje statičkih fajlova
app.UseDefaultFiles(); // traži index.html
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Run();
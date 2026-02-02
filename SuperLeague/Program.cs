using SuperLeague.ExternalAPI.Interfaces;
using SuperLeague.ExternalAPI.Services;
using SuperLeague.Interfaces;
using SuperLeague.Repositories;
using SuperLeague.Services;
using SuperLeague.Services.Sync;

var builder = WebApplication.CreateBuilder(args);

// Registracija kontrolera
builder.Services.AddControllers();

// HttpClient for FootballAPI
builder.Services.AddHttpClient<IFootballApiService, FootballApiService>((services, client) =>
{
    var confing = services.GetRequiredService<IConfiguration>();
    var apiKey = confing["FootballApi:Key"];

    if (!string.IsNullOrEmpty(apiKey))
    {
        client.DefaultRequestHeaders.Add("x-apisports-key", apiKey); 
    }
});

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//  Register Repositories
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ILeagueRepository, LeagueRepository>();

// Register Services
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IDataSyncService, DataSyncService>();

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
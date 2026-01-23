using Dapper;
using Microsoft.Data.SqlClient;
using SuperLeague.Interfaces;
using SuperLeague.Models;
using System.Data;

namespace SuperLeague.Repositories
{
    public class LeagueRepository : ILeagueRepository
    {
        private readonly string _connectionString;

        public LeagueRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);

        public async Task<IEnumerable<LeagueTable_Team>> GetLeagueTableAsync(int seasonId)
        {
            using var db = Connection;
            var query = @"
                SELECT * FROM LeagueTable_Team 
                WHERE SeasonID = @SeasonId 
                ORDER BY Points DESC, (GoalsFor - GoalsAgainst) DESC";
            return await db.QueryAsync<LeagueTable_Team>(query, new { SeasonId = seasonId });
        }

        public async Task<IEnumerable<PlayerStats>> GetTopScorersAsync(int seasonId, int count)
        {
            using var db = Connection;
            var query = @"
                SELECT TOP (@Count) 
                    P.PlayerFirstName, P.PlayerLastName, T.TeamName, 
                    SUM(MP.Goals) as TotalGoals
                FROM Match_Player MP
                JOIN Player P ON MP.PlayerID = P.PlayerID
                JOIN Team T ON MP.TeamID = T.TeamID
                JOIN Match M ON MP.MatchID = M.MatchID
                WHERE M.SeasonID = @SeasonId
                GROUP BY P.PlayerFirstName, P.PlayerLastName, T.TeamName
                ORDER BY TotalGoals DESC";
            return await db.QueryAsync<PlayerStats>(query, new { SeasonId = seasonId, Count = count });
        }

        public async Task<IEnumerable<PlayerStats>> GetTopAssistersAsync(int seasonId, int count)
        {
            using var db = Connection;
            var query = @"
                SELECT TOP (@Count) 
                    P.PlayerFirstName, P.PlayerLastName, T.TeamName, 
                    SUM(MP.Assists) as TotalAssists
                FROM Match_Player MP
                JOIN Player P ON MP.PlayerID = P.PlayerID
                JOIN Team T ON MP.TeamID = T.TeamID
                JOIN Match M ON MP.MatchID = M.MatchID
                WHERE M.SeasonID = @SeasonId
                GROUP BY P.PlayerFirstName, P.PlayerLastName, T.TeamName
                ORDER BY TotalAssists DESC";
            return await db.QueryAsync<PlayerStats>(query, new { SeasonId = seasonId, Count = count });
        }
    }
}

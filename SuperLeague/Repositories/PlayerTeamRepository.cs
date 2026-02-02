using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using SuperLeague.Domain.Models;
using SuperLeague.Interfaces;
using System.Data;

namespace SuperLeague.Repositories
{
    public class PlayerTeamRepository : IPlayerTeamRepository
    {
        private readonly string _connectionString;

        public PlayerTeamRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<int> AddAsync(PlayerTeam playerTeam)
        {
            using var connection = CreateConnection();

            var query = @"
                INSERT INT Player_Team  (PlayerId, TeamId, StartDate, EndDate)
                OUTPUT INSERTED.Player_TeamID
                VALUES (@PlayerId, @TeamId, @StartDate, @EndDate)";

            return await connection.ExecuteScalarAsync<int>(query, playerTeam);
        }
        public async Task<bool> ExistsAsync(int playerId, int teamId)
        {
            using var connection = CreateConnection();

            var query = @"
                SELECT COUNT(1)
                FFROM Player_Team
                WHERE PlayerId = @PlayerId AND TeamId AND EndDate IS NULL";

            var count = await connection.ExecuteScalarAsync<int>(query, new { PlayerId = playerId, TeamId = teamId});
            return count > 0;
        }
        public async Task<PlayerTeam?> GetActiveByPlayerIdAsync(int playerId)
        {
            using var connection = CreateConnection();

            var query = @" SELECT 
                                Player_TeamID as PlayerTeamId,
                                PlayerId,
                                TeamId,
                                StartDate,
                                EndDate
                           FROM Player_Team
                           WHERE PlayerId = @PlayerId AND EndDate IS NULL
                           ORDER BY StartDate DESC";
            return await connection.QueryFirstOrDefaultAsync<PlayerTeam?>(query, new {PlayerId = playerId});
        }
        public async Task UpdateAsync(PlayerTeam playerTeam)
        {
            using var connection = CreateConnection();

            var query = @"UPDATE Player_Team
                          SET EndDate = @EndDate
                          WHERE Player_TeamID = @PlayerTeamId";

            await connection.ExecuteAsync(query, playerTeam)    ;

        }
    }
}

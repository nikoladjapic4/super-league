// Repositories/PlayerTeamRepository.cs
using Dapper;
using Microsoft.Data.SqlClient;
using SuperLeague.Domain.Models;
using SuperLeague.Interfaces.Repository;
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

            var sql = @"
                INSERT INTO Player_Team (PlayerId, TeamId, StartDate, EndDate)
                OUTPUT INSERTED.Player_TeamID
                VALUES (@PlayerId, @TeamId, @StartDate, @EndDate)";

            return await connection.ExecuteScalarAsync<int>(sql, playerTeam);
        }

        public async Task<bool> ExistsAsync(int playerId, int teamId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT COUNT(1)
                FROM Player_Team
                WHERE PlayerId = @PlayerId 
                AND TeamId = @TeamId
                AND EndDate IS NULL"; // Aktivna veza

            var count = await connection.ExecuteScalarAsync<int>(sql, new { PlayerId = playerId, TeamId = teamId });
            return count > 0;
        }

        public async Task<PlayerTeam?> GetActiveByPlayerIdAsync(int playerId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    Player_TeamID as PlayerTeamId,
                    PlayerId,
                    TeamId,
                    StartDate,
                    EndDate
                FROM Player_Team
                WHERE PlayerId = @PlayerId 
                AND EndDate IS NULL
                ORDER BY StartDate DESC";

            return await connection.QueryFirstOrDefaultAsync<PlayerTeam>(sql, new { PlayerId = playerId });
        }

        public async Task UpdateAsync(PlayerTeam playerTeam)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Player_Team
                SET EndDate = @EndDate
                WHERE Player_TeamID = @PlayerTeamId";

            await connection.ExecuteAsync(sql, playerTeam);
        }

        public async Task<List<PlayerTeam>> GetAllActiveLinksAsync()
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    Player_TeamID as PlayerTeamId,
                    PlayerId,
                    TeamId,
                    StartDate,
                    EndDate
                FROM Player_Team
                WHERE EndDate IS NULL";

            var result = await connection.QueryAsync<PlayerTeam>(sql);
            return result.ToList();
        }

        public async Task BulkAddAsync(List<PlayerTeam> playerTeams)
        {
            using var connection = CreateConnection();

            var sql = @"
                INSERT INTO Player_Team (PlayerId, TeamId, StartDate, EndDate)
                VALUES (@PlayerId, @TeamId, @StartDate, @EndDate)";

            await connection.ExecuteAsync(sql, playerTeams);
        }
    }

}
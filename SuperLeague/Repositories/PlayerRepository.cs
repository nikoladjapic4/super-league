using Dapper;
using Microsoft.Data.SqlClient;
using SuperLeague.Interfaces;
using SuperLeague.Models;
using System.Data;

namespace SuperLeague.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly string _connectionString;

        public PlayerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Player>> GetAllActiveAsync()
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    PlayerId,
                    PlayerFirstName,
                    PlayerLastName,
                    JerseyNumber,
                    Nationality,
                    Position,
                    BirthDate,
                    IsActive,
                    CreatedAt,
                    CreatedBy,
                    LockedAt,
                    LockedBy,
                    DeletedAt,
                    DeletedBy,
                    VersionPlayer
                FROM Player
                WHERE IsActive = 1
                ORDER BY PlayerLastName, PlayerFirstName";

            return await connection.QueryAsync<Player>(sql);
        }

        public async Task<Player?> GetByIdAsync(int playerId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    PlayerId,
                    PlayerFirstName,
                    PlayerLastName,
                    JerseyNumber,
                    Nationality,
                    Position,
                    BirthDate,
                    IsActive,
                    CreatedAt,
                    CreatedBy,
                    LockedAt,
                    LockedBy,
                    DeletedAt,
                    DeletedBy,
                    VersionPlayer
                FROM Player
                WHERE PlayerId = @PlayerId";

            return await connection.QueryFirstOrDefaultAsync<Player>(sql, new { PlayerId = playerId });
        }

        public async Task<int> AddAsync(Player player)
        {
            using var connection = CreateConnection();

            var sql = @"
                INSERT INTO Player 
                (PlayerFirstName, PlayerLastName, JerseyNumber, Nationality, Position, BirthDate, 
                 CreatedAt, CreatedBy, IsActive)
                OUTPUT INSERTED.PlayerId
                VALUES 
                (@PlayerFirstName, @PlayerLastName, @JerseyNumber, @Nationality, @Position, @BirthDate,
                 @CreatedAt, @CreatedBy, @IsActive)";

            return await connection.ExecuteScalarAsync<int>(sql, player);
        }

        public async Task<bool> UpdateAsync(Player player)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Player 
                SET 
                    PlayerFirstName = @PlayerFirstName,
                    PlayerLastName = @PlayerLastName,
                    JerseyNumber = @JerseyNumber,
                    Nationality = @Nationality,
                    Position = @Position,
                    BirthDate = @BirthDate,
                    IsActive = @IsActive,
                    DeletedAt = @DeletedAt,
                    DeletedBy = @DeletedBy,
                    LockedAt = @LockedAt,
                    LockedBy = @LockedBy
                WHERE PlayerId = @PlayerId 
                AND VersionPlayer = @VersionPlayer";

            var rowsAffected = await connection.ExecuteAsync(sql, player);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string firstName, string lastName, DateTime birthDate, int? excludePlayerId = null)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT COUNT(1) 
                FROM Player
                WHERE PlayerFirstName = @FirstName 
                AND PlayerLastName = @LastName 
                AND BirthDate = @BirthDate
                AND IsActive = 1
                AND (@ExcludePlayerId IS NULL OR PlayerId != @ExcludePlayerId)";

            var count = await connection.ExecuteScalarAsync<int>(sql,
                new
                {
                    FirstName = firstName,
                    LastName = lastName,
                    BirthDate = birthDate,
                    ExcludePlayerId = excludePlayerId
                });

            return count > 0;
        }

    }
}
// Repositories/TeamRepository.cs
using Dapper;
using Microsoft.Data.SqlClient;
using SuperLeague.Interfaces;
using SuperLeague.Models;
using System.Data;

namespace SuperLeague.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private readonly string _connectionString;

        public TeamRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<IEnumerable<Team>> GetAllActiveAsync()
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    TeamId,
                    TeamName,
                    DateOfFoundation,
                    Stadium,
                    City,
                    IsActive,
                    CreatedAt,
                    CreatedBy,
                    UpdatedAt,
                    UpdatedBy,
                    DeletedAt,
                    DeletedBy,
                    VersionTeam as VersionRow,
                    LockedAt,
                    LockedBy
                FROM Team
                WHERE IsActive = 1
                ORDER BY TeamName";

            return await connection.QueryAsync<Team>(sql);
        }

        public async Task<Team?> GetByIdAsync(int teamId)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT 
                    TeamId,
                    TeamName,
                    DateOfFoundation,
                    Stadium,
                    City,
                    IsActive,
                    CreatedAt,
                    CreatedBy,
                    UpdatedAt,
                    UpdatedBy,
                    DeletedAt,
                    DeletedBy,
                    VersionTeam as VersionRow,
                    LockedAt,
                    LockedBy
                FROM Team
                WHERE TeamId = @TeamId";

            return await connection.QueryFirstOrDefaultAsync<Team>(sql, new { TeamId = teamId });
        }

        public async Task<int> AddAsync(Team team)
        {
            using var connection = CreateConnection();

            var sql = @"
                INSERT INTO Team 
                (TeamName, DateOfFoundation, Stadium, City, CreatedAt, CreatedBy, IsActive)
                OUTPUT INSERTED.TeamId
                VALUES 
                (@TeamName, @DateOfFoundation, @Stadium, @City, @CreatedAt, @CreatedBy, @IsActive)";

            return await connection.ExecuteScalarAsync<int>(sql, team);
        }

        public async Task<bool> UpdateAsync(Team team)
        {
            using var connection = CreateConnection();

            var sql = @"
                UPDATE Team 
                SET 
                    TeamName = @TeamName,
                    DateOfFoundation = @DateOfFoundation,
                    Stadium = @Stadium,
                    City = @City,
                    UpdatedAt = @UpdatedAt,
                    UpdatedBy = @UpdatedBy
                WHERE TeamId = @TeamId 
                AND VersionTeam = @VersionRow
                AND IsActive = 1";

            var rowsAffected = await connection.ExecuteAsync(sql, team);
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(string teamName, string city, int? excludeTeamId = null)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT COUNT(1) 
                FROM Team
                WHERE TeamName = @TeamName 
                AND City = @City 
                AND IsActive = 1
                AND (@ExcludeTeamId IS NULL OR TeamId != @ExcludeTeamId)";

            var count = await connection.ExecuteScalarAsync<int>(sql,
                new { TeamName = teamName, City = city, ExcludeTeamId = excludeTeamId });

            return count > 0;
        }
    }
}
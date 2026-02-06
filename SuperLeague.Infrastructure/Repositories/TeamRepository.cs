using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SuperLeague.Domain.Interfaces.Repositories;
using SuperLeague.Domain.Entities;
using System.Data;

namespace SuperLeague.Infrastructure.Repositories
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
                INSERT INTO Team (TeamName, DateOfFoundation, Stadium, City, CreatedAt, CreatedBy, IsActive)
                OUTPUT INSERTED.TeamId, INSERTED.VersionTeam
                VALUES (@TeamName, @DateOfFoundation, @Stadium, @City, @CreatedAt, @CreatedBy, @IsActive)
            ";

            var result = await connection.QuerySingleAsync<(int TeamId, byte[] VersionRow)>(sql, team);

            team.TeamId = result.TeamId;
            team.VersionRow = result.VersionRow; // OSVEŽI stvarnu rowversion iz baze
            return team.TeamId;
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
                    IsActive = @IsActive
                WHERE TeamId = @TeamId AND VersionTeam = @VersionRow";

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
        public async Task<Dictionary<string, int>> GetColumnLengthsAsync()
        {
            using var connection = CreateConnection();
            var sql = @"
                SELECT COLUMN_NAME, CHARACTER_MAXIMUM_LENGTH 
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'Team' AND DATA_TYPE IN ('varchar', 'nvarchar', 'char', 'nchar')";
            
            var result = await connection.QueryAsync<(string ColumnName, int? MaxLength)>(sql);
            
            // Filter out null MaxLengths (e.g. invalid types) and return dictionary
            return result
                .Where(x => x.MaxLength.HasValue)
                .ToDictionary(x => x.ColumnName, x => x.MaxLength!.Value);
        }
    }
}
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
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);
        public async Task<IEnumerable<Team>> GetAllAsync()
        {
            using var db = Connection;
            var query = @"SELECT TeamID, TeamName, DateOfFoundation,
                                  IsActive, CreatedAt, CreatedBy, LockedAt, LockedBy, DeletedAt, DeletedBy, VersionTeam AS VersionRow, Stadium, City
                          FROM Team";
            return await db.QueryAsync<Team>(query);
        }

        public async Task<Team?> GetByIdAsync(int teamId)
        {
            using var db = Connection;
            var query = @"SELECT TeamID, TeamName, DateOfFoundation, 
                                  IsActive, CreatedAt, CreatedBy, LockedAt, LockedBy, DeletedAt, DeletedBy, VersionTeam AS VersionRow,  Stadium, City
                          FROM Team 
                          WHERE TeamId = @TeamId";
            return await db.QueryFirstOrDefaultAsync<Team>(query, new { TeamId = teamId });
        }
        public async Task AddAsync(Team team)
        {
            using var db = Connection;
            team.CreatedAt = DateTime.UtcNow;
            team.IsActive = true;

            var query = @"INSERT INTO Team 
                            (TeamName, DateOfFoundation,Stadium, City, CreatedAt, CreatedBy, IsActive)
                          VALUES    
                            (@TeamName, @DateOfFoundation, @Stadium, @City, @CreatedAt, @CreatedBy, @IsActive)";
            await db.ExecuteAsync(query, team);
        }

        public async Task<bool> UpdateAsync(Team team)
        {
            using var db = Connection;
            
            var query = @"UPDATE Team 
                          SET TeamName = @TeamName, DateOfFoundation = @DateOfFoundation,Stadium = @Stadium, City = @City, LockedAt = NULL, LockedBy = NULL 
                          WHERE TeamId = @TeamId AND VersionTeam = @VersionRow";
            var affected = await db.ExecuteAsync(query, team);
            return affected > 0;
        }

        public async Task<bool> SoftDeleteAsync(int teamId, byte[] versionRow, int deletedBy)
        {
            using var db = Connection;
            var query = @"UPDATE Team 
                          SET IsActive = 0, DeletedAt = GETUTCDATE(), DeletedBy = @DeletedBy
                         WHERE TeamId = @TeamId AND VersionTeam = @VersionRow AND IsActive = 1";
            var affected = await db.ExecuteAsync(query, new { TeamId = teamId, VersionRow = versionRow, DeletedBy = deletedBy });
            return affected > 0;
        }

        public async Task<bool> RestoreAsync(int teamId)
        {
            using var db = Connection;
            var query = @"UPDATE Team
                          SET IsActive = 1, DeletedAt = NULL, DeletedBy = NULL
                          WHERE TeamId = @TeamId AND IsActive = 0";
            var affected = await db.ExecuteAsync(query, new {TeamId = teamId});
            return affected > 0;
        }

        public async Task<bool> TeamExistingAsync(string teamName, string city)
        {
            using var db = Connection;
            var query = @"SELECT COUNT(*) FROM Team
                           WHERE TeamName = @TeamName AND City = @City AND IsActive = 1";
            var count = await db.ExecuteScalarAsync<int>(query, new { TeamName = teamName, City = city });
            return count > 0;
        }
    }
}

using Microsoft.Data.SqlClient;
using SuperLeague.Interfaces;
using SuperLeague.Models;
using System.Data;
using Dapper;
using SuperLeague.DTOs;

namespace SuperLeague.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly string _connectionString;
        public PlayerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection Connection => new SqlConnection(_connectionString);


        public async Task<IEnumerable<Player>> GetAllAsync(int teamId)
        {
            using var db = Connection;
            var query = @"
                        SELECT 
                            P.PlayerID,
                            P.JerseyNumber,
                            P.PlayerFirstName,
                            P.PlayerLastName,
                            P.Nationality,
                            P.Position,
                            P.BirthDate
                        FROM Player_Team PT
                        JOIN Player P ON P.PlayerID = PT.PlayerID
                        JOIN Team T ON T.TeamID = PT.TeamID
                        WHERE T.TeamID = @TeamID
                        ORDER BY 
                            CASE P.Position 
                                WHEN 'Goalkeeper' THEN 1  
                                WHEN 'Defender' THEN 2 
                                WHEN 'Midfielder' THEN 3  
                                WHEN 'Forward' THEN 4  
                                ELSE 5  
                            END, 
                            P.JerseyNumber;";
            return await db.QueryAsync<Player>(query, new { TeamId = teamId });
             
        }

      
       

        public async Task<IEnumerable<PlayerStats>> GetStatsByIdAsync(int playerId)
        {
            using var db = Connection;
            var query = @"
                SELECT 
                    S.SeasonName,
                    P.PlayerID,
                    P.PlayerFirstName,
                    P.PlayerLastName,
                    LTT.TeamName,
                    SUM(MP.Goals) AS TotalGoals,
                    SUM(MP.Assists) AS TotalAssists,
                    SUM(MP.YellowCards) AS TotalYellows,
                    SUM(MP.RedCards) AS TotalReds,
                    SUM(MP.MinutesPlayed) AS TotalMinutes,
                    COUNT(DISTINCT MP.MatchID) AS MatchesPlayed
                FROM Match_Player MP
                INNER JOIN Player P ON MP.PlayerID = P.PlayerID
                INNER JOIN Team T ON MP.TeamID = T.TeamID
                INNER JOIN Match M ON MP.MatchID = M.MatchID
                INNER JOIN Season S ON M.SeasonID = S.SeasonID
                INNER JOIN LeagueTable_Team LTT ON LTT.TeamID = T.TeamID AND LTT.SeasonID = S.SeasonID
                WHERE MP.PlayerID = @PlayerID
                GROUP BY S.SeasonName, LTT.TeamName, P.PlayerFirstName, P.PlayerLastName, T.TeamID, P.PlayerID
                ORDER BY S.SeasonName, T.TeamID;";
            return await db.QueryAsync<PlayerStats>(query, new { PlayerId = playerId });
        }

        public async Task AddAsync(CreatePlayerDto dto, int teamId)
        {
            using var db = new SqlConnection(_connectionString);
            await db.OpenAsync();
            using var transaction = db.BeginTransaction();

            try
            {
                
                var existingPlayerId = await GetExistingPlayerIdAsync(dto, db, transaction);
                int playerId;

                if (existingPlayerId.HasValue)
                {
                    playerId = existingPlayerId.Value;
                }
                else
                {
                    var player = new Player
                    {
                        PlayerFirstName = dto.PlayerFirstName,
                        PlayerLastName = dto.PlayerLastName,
                        JerseyNumber = dto.JerseyNumber,
                        Position = dto.Position,
                        BirthDate = dto.BirthDate,
                        Nationality = dto.Nationality,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1,
                        IsActive = true
                        
                    };

                    var insertQuery = @"
                INSERT INTO Player 
                        (PlayerFirstName, PlayerLastName, JerseyNumber, Position, BirthDate, CreatedAt, CreatedBy, IsActive, Nationality, LockedBy)
                OUTPUT INSERTED.PlayerID
                VALUES 
                        (@PlayerFirstName, @PlayerLastName, @JerseyNumber, @Position, @BirthDate, @CreatedAt, @CreatedBy, @IsActive, @Nationality, @LockedBy);";

                    playerId = await db.ExecuteScalarAsync<int>(insertQuery, player, transaction);
                }

                // Provera da li je već dodeljen timu
                var existingRelation = await db.QueryFirstOrDefaultAsync<int?>(@"
            SELECT 1 FROM Player_Team
            WHERE PlayerID = @PlayerId AND TeamID = @TeamId",
                    new { PlayerId = playerId, TeamId = teamId }, transaction);

                if (existingRelation != null)
                {
                    throw new Exception("Player is already assigned to this team.");
                }

                // Veza igrač-tim
                var insertRelationQuery = @"
            INSERT INTO Player_Team (PlayerID, TeamID, StartDate)
            VALUES (@PlayerID, @TeamID, @StartDate);";

                await db.ExecuteAsync(insertRelationQuery, new
                {
                    PlayerID = playerId,
                    TeamID = teamId,
                    StartDate = DateTime.UtcNow
                }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }


        public async Task<int?> GetExistingPlayerIdAsync(CreatePlayerDto dto, SqlConnection db,SqlTransaction transaction)
        {
            return await db.QueryFirstOrDefaultAsync<int?>
                (@"SELECT PlayerID FROM Player
                   WHERE  PlayerFirstName = @PlayerFirstName
                    AND PlayerLastName = @PlayerLastName
                    AND BirthDate = @BirthDate",
                    new
                    {
                        dto.PlayerFirstName,
                        dto.PlayerLastName,
                        dto.BirthDate
                    }, transaction);
        }

    }
}

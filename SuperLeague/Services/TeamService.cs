using SuperLeague.DTOs.Team;
using SuperLeague.Exceptions;
using SuperLeague.Interfaces;
using SuperLeague.Models;

namespace SuperLeague.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IExternalApiService _externalApiService;
        private readonly ILogger<TeamService> _logger;

        public TeamService(
            ITeamRepository teamRepository,
            IExternalApiService externalApiService,
            ILogger<TeamService> logger)
        {
            _teamRepository = teamRepository;
            _externalApiService = externalApiService;
            _logger = logger;
        }

        public async Task<IEnumerable<TeamDto>> GetAllTeamsAsync()
        {
            var teams = await _teamRepository.GetAllActiveAsync();
            return teams.Select(MapToDto);
        }

        public async Task<TeamDto> GetTeamByIdAsync(int teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);

            if (team == null || !team.IsActive)
            {
                throw new NotFoundException($"Tim sa ID-om {teamId} nije pronađen");
            }

            return MapToDto(team);
        }

        public async Task<TeamDto> CreateTeamAsync(CreateTeamDto dto, int createdBy)
        {
            // Provera da li tim već postoji
            var exists = await _teamRepository.ExistsAsync(dto.TeamName, dto.City);
            if (exists)
            {
                throw new BusinessRuleException(
                    $"Tim '{dto.TeamName}' u gradu '{dto.City}' već postoji");
            }
            if (string.IsNullOrWhiteSpace(dto.TeamName))
            {
                throw new BusinessRuleException("Naziv tima je obavezan");
            }


            var team = new Team
            {
                TeamName = dto.TeamName,
                DateOfFoundation = dto.DateOfFoundation,
                Stadium = dto.Stadium,
                City = dto.City,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                IsActive = true
            };

            var newTeamId = await _teamRepository.AddAsync(team);
            team.TeamId = newTeamId;

            _logger.LogInformation("Tim kreiran: {TeamId} - {TeamName}", newTeamId, team.TeamName);

            return MapToDto(team);
        }

        public async Task<TeamDto> UpdateTeamAsync(int teamId, UpdateTeamDto dto, int updatedBy)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null || !team.IsActive)
            {
                throw new NotFoundException($"Tim sa ID-om {teamId} nije pronađen");
            }

            // Provera da li je tim zaključan
            if (team.LockedAt.HasValue && team.LockedBy.HasValue && team.LockedBy != updatedBy)
            {
                throw new TeamLockedException(
                    $"Tim je trenutno zaključan od strane drugog korisnika (ID: {team.LockedBy})");
            }

            // Provera duplikata (isključujući trenutni tim)
            var exists = await _teamRepository.ExistsAsync(dto.TeamName, dto.City, teamId);
            if (exists)
            {
                throw new BusinessRuleException(
                    $"Drugi tim sa imenom '{dto.TeamName}' u gradu '{dto.City}' već postoji");
            }

            // Ažuriranje polja
            team.TeamName = dto.TeamName;
            team.DateOfFoundation = dto.DateOfFoundation;
            team.Stadium = dto.Stadium;
            team.City = dto.City;
            team.VersionRow = dto.VersionRow; 

            var success = await _teamRepository.UpdateAsync(team);
            if (!success)
            {
                throw new ConcurrencyException(
                    "Tim je izmenjen od strane drugog korisnika. Molimo osvežite stranicu i pokušajte ponovo.");
            }

            _logger.LogInformation("Tim ažuriran: {TeamId} - {TeamName}", teamId, team.TeamName);

            // Učitaj ažurirani tim (sa novim VersionRow)
            var updatedTeam = await _teamRepository.GetByIdAsync(teamId);
            return MapToDto(updatedTeam!);
        }

        public async Task<bool> DeleteTeamAsync(int teamId, int deletedBy)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                throw new NotFoundException($"Tim sa ID-om {teamId} nije pronađen");
            }

            if (!team.IsActive)
            {
                throw new BusinessRuleException("Tim je već obrisan");
            }

            // Provera da li je tim zaključan
            if (team.LockedAt.HasValue && team.LockedBy.HasValue && team.LockedBy != deletedBy)
            {
                throw new TeamLockedException(
                    $"Tim je trenutno zaključan od strane drugog korisnika");
            }

            // Soft delete
            team.IsActive = false;
            team.DeletedAt = DateTime.UtcNow;
            team.DeletedBy = deletedBy;

            var success = await _teamRepository.UpdateAsync(team);

            if (success)
            {
                _logger.LogInformation("Tim soft obrisan: {TeamId}", teamId);
            }

            return success;
        }

        public async Task<bool> RestoreTeamAsync(int teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null)
            {
                throw new NotFoundException($"Tim sa ID-om {teamId} nije pronađen");
            }

            if (team.IsActive)
            {
                throw new BusinessRuleException("Tim je već aktivan");
            }

            team.IsActive = true;
            team.DeletedAt = null;
            team.DeletedBy = null;

            var success = await _teamRepository.UpdateAsync(team);

            if (success)
            {
                _logger.LogInformation("Tim vraćen: {TeamId}", teamId);
            }

            return success;
        }

        public async Task LockTeamAsync(int teamId, int lockedBy)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null || !team.IsActive)
            {
                throw new NotFoundException($"Tim sa ID-om {teamId} nije pronađen");
            }
            if (team.LockedAt.HasValue &&
                    team.LockedAt.Value < DateTime.UtcNow.AddMinutes(-15))
            {
                team.LockedAt = null;
                team.LockedBy = null;
            }

            if (team.LockedBy.HasValue)
            {
                throw new TeamLockedException(
                    $"Tim je već zaključan od strane korisnika (ID: {team.LockedBy})");
            }

            team.LockedAt = DateTime.UtcNow;
            team.LockedBy = lockedBy;

            await _teamRepository.UpdateAsync(team);

            _logger.LogInformation("Tim zaključan: {TeamId} od strane {UserId}", teamId, lockedBy);
        }

        public async Task UnlockTeamAsync(int teamId, int userId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null || !team.IsActive)
            {
                throw new NotFoundException($"Tim sa ID-om {teamId} nije pronađen");
            }

            if (!team.LockedAt.HasValue)
            {
                throw new BusinessRuleException("Tim nije zaključan");
            }

            // Samo korisnik koji je zaključao može otključati (ili admin u budućnosti)
            if (team.LockedBy != userId)
            {
                throw new BusinessRuleException(
                    "Samo korisnik koji je zaključao tim može ga otključati");
            }

            team.LockedAt = null;
            team.LockedBy = null;

            await _teamRepository.UpdateAsync(team);

            _logger.LogInformation("Tim otključan: {TeamId}", teamId);
        }

        public async Task SyncTeamsAsync(int leagueId, int season)
        {
            _logger.LogInformation("Započinjem sinhronizaciju timova za ligu {LeagueId}, sezona {Season}", leagueId, season);

            // Fetch actual DB column limits
            var columnLimits = await _teamRepository.GetColumnLengthsAsync();
            var teamNameLimit = columnLimits.GetValueOrDefault("TeamName", 50);
            var cityLimit = columnLimits.GetValueOrDefault("City", 50);
            var stadiumLimit = columnLimits.GetValueOrDefault("Stadium", 50);

            _logger.LogInformation("Column limits: TeamName={TeamNameLimit}, City={CityLimit}, Stadium={StadiumLimit}", 
                teamNameLimit, cityLimit, stadiumLimit);

            var apiTeams = await _externalApiService.GetTeamsByLeagueAsync(leagueId, season);

            foreach (var apiTeam in apiTeams)
            {
                var teamName = Truncate(apiTeam.Team.Name, teamNameLimit);
                var city = Truncate(apiTeam.Venue.City, cityLimit);

                // Provera da li tim već postoji u bazi (koristimo postojeći ExistsAsync)
                var exists = await _teamRepository.ExistsAsync(teamName, city);

                if (!exists)
                {
                    var newTeam = new Team
                    {
                        TeamName = teamName,
                        City = city,
                        Stadium = Truncate(apiTeam.Venue.Name, stadiumLimit),
                        DateOfFoundation = apiTeam.Team.Founded.HasValue 
                            ? new DateTime(apiTeam.Team.Founded.Value, 1, 1) 
                            : new DateTime(1900, 1, 1),
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = 1, // System/Admin
                        IsActive = true
                    };

                    await _teamRepository.AddAsync(newTeam);
                    _logger.LogInformation("Dodat novi tim: {TeamName}", teamName);
                }
                else
                {
                    _logger.LogInformation("Tim već postoji: {TeamName}", teamName);
                    // Ovde bi mogao dodati i update logiku ako želiš da osvežavaš podatke
                }
            }

            _logger.LogInformation("Sinhronizacija završena.");
        }

        private static string Truncate(string? value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private static TeamDto MapToDto(Team team)
        {
            return new TeamDto
            {
                TeamId = team.TeamId,
                TeamName = team.TeamName,
                DateOfFoundation = team.DateOfFoundation,
                Stadium = team.Stadium,
                City = team.City,
                IsActive = team.IsActive,
                CreatedAt = team.CreatedAt,
                VersionRow = team.VersionRow
            };
        }
    }
}
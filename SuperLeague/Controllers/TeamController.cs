using Microsoft.AspNetCore.Mvc;
using SuperLeague.DTOs;
using SuperLeague.Interfaces;
using SuperLeague.Models;


namespace SuperLeague.Controllers
{
    [ApiController] //automatski vraca 400 bad request, parsira JSON iz tela u c# objekat
    [Route("api/[controller]")] //svi endpointi pocinju sa api/ ime iz _____Controller u mom slucaju team

    public class TeamController : ControllerBase //ControllerBase sluzi za webapi , moze da vraca 
    {
        private readonly ITeamRepository _teamRepository;

        public TeamController(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var teams = await _teamRepository.GetAllAsync();
            return Ok(teams);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var team = await _teamRepository.GetByIdAsync(id);
            return team == null ? NotFound() : Ok(team);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeamDto dto)
        {
            var exists = await _teamRepository.TeamExistingAsync(dto.TeamName, dto.City);
            if (exists)
            {
                return Conflict("Team already exists.");
            }

            var team = new Team
            {
                TeamName = dto.TeamName,
                DateOfFoundation = dto.DateOfFoundation,
                Stadium = dto.Stadium,
                City = dto.City,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = 1,
                IsActive = true

            };


            await _teamRepository.AddAsync(team);
            return Ok(new { message = "Team created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamDto dto)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
            {
                return NotFound();
            }

            if (existingTeam.LockedAt != null)
                return Conflict($"Team is currently locked by someone else. Locked at: {existingTeam.LockedAt}");

            if (existingTeam.VersionRow.SequenceEqual(dto.VersionRow))
            {
                existingTeam.TeamName = dto.TeamName;
                existingTeam.DateOfFoundation = dto.DateOfFoundation;
                existingTeam.Stadium = dto.Stadium;
                existingTeam.City = dto.City;


                var success = await _teamRepository.UpdateAsync(existingTeam);
                if (!success)
                {
                    return Conflict("Team has been locked by someone else.");
                }

                return Ok("Team is successfully updated.");
            }

            return Conflict("This data has been modified by someone else. Please refresh and try again.");
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {

            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                return NotFound();

            if (team.IsActive == false)
                return Conflict("Team is already deleted.");

            if (team.LockedAt != null)
                return Conflict($"Team '{team.TeamName}' is currently locked by someone else. Locked at: {team.LockedAt}");

            var success = await _teamRepository.SoftDeleteAsync(id, team.VersionRow, 1);

            if (!success)
            {
                return Conflict($"Couldn't delete team '{team.TeamName}'. Team is locked by someone else.");
            }
            return Ok("Team is successfully deleted.");
        }




        [HttpPatch("{id}")]
        public async Task<IActionResult> AddTeam(int id)
        {
            var restored = await _teamRepository.RestoreAsync(id);
            if (!restored)
                return NotFound("Team not found or already active.");
            return Ok("Team successfully added back.");
        }



    }
}
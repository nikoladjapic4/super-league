using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create([FromBody] Team team)
        {
            var exists = await _teamRepository.TeamExistingAsync(team.TeamName, team.City);
            if (exists)
            {
                return Conflict("Team already exists.");
            }

            team.CreatedAt = DateTime.UtcNow;
            team.CreatedBy = 1;

            await _teamRepository.AddAsync(team);
            return Ok(new {message = "Team created successfully."});
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Team updatedTeam)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(id);
            if (existingTeam == null)
                return NotFound();

            if (existingTeam.LockedAt != null)
                return Conflict("Team is currently locked by someone else.");
            
            updatedTeam.TeamId = id;
            updatedTeam.VersionRow = existingTeam.VersionRow;
            updatedTeam.LockedAt = DateTime.UtcNow;
            
            
            
            var success = await _teamRepository.UpdateAsync(updatedTeam);
            if (!success)
                return Conflict("Team has been locked by someone else.");
            return Ok("Team is successfully updated.");
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Delete(int id)
        {
            
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null)
                return NotFound();

            if(team.LockedAt != null)
                return Conflict("Team has been locked by someone else.");

            await _teamRepository.SoftDeleteAsync(id, team.VersionRow, 1);
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


        /*
        [HttpGet]
        public IActionResult GetAllTeams()
        {
            var teams = new[]
            {
                new { Id = 1, TeamName = "Red Star" },
                new { Id = 2, TeamName = "Partizan" }
            };

            return Ok(teams);
        }

        [HttpGet("{teamId}")]
        public IActionResult GetTeam(int teamId)
        {
            var teams = new[]
            {
            new { Id = 1, TeamName = "Red Star" },
            new { Id = 2, TeamName = "Partizan" }
        };

            var team = teams.FirstOrDefault(t => t.Id == teamId);

            if (team == null)
                return NotFound();

            return Ok(team);
        }
        */
    }
}
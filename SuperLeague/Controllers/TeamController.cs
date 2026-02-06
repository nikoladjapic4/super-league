using Microsoft.AspNetCore.Mvc;
using SuperLeague.Application.DTOs.Team;
using SuperLeague.Application.Exceptions;
using SuperLeague.Application.Services.Interfaces;

namespace SuperLeague.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly ILogger<TeamController> _logger;

        public TeamController(
            ITeamService teamService,
            ILogger<TeamController> logger)
        {
            _teamService = teamService;
            _logger = logger;
        }

        /// <summary>
        /// Vraća sve aktivne timove
        /// GET: api/team
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var teams = await _teamService.GetAllTeamsAsync();
                return Ok(teams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom preuzimanja svih timova");
                return StatusCode(500, new { message = "Došlo je do greške prilikom preuzimanja timova" });
            }
        }

        /// <summary>
        /// Vraća tim po ID-u
        /// GET: api/team/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var team = await _teamService.GetTeamByIdAsync(id);
                return Ok(team);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom preuzimanja tima {TeamId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom preuzimanja tima" });
            }
        }

        /// <summary>
        /// Kreira novi tim
        /// POST: api/team
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeamDto dto)
        {
            try
            {
                // TODO: Kada dodaš autentifikaciju, uzmi stvarni ID korisnika
                int createdBy = 1; // Hardcoded za sada

                var team = await _teamService.CreateTeamAsync(dto, createdBy);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = team.TeamId },
                    new { message = "Tim je uspešno kreiran", data = team }
                );
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom kreiranja tima");
                return StatusCode(500, new { message = "Došlo je do greške prilikom kreiranja tima" });
            }
        }

        /// <summary>
        /// Ažurira postojeći tim
        /// PUT: api/team/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamDto dto)
        {
            try
            {
                // TODO: Kada dodaš autentifikaciju, uzmi stvarni ID korisnika
                int updatedBy = 1; // Hardcoded za sada

                var team = await _teamService.UpdateTeamAsync(id, dto, updatedBy);

                return Ok(new { message = "Tim je uspešno ažuriran", data = team });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ConcurrencyException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (TeamLockedException ex) // Nova exception
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom ažuriranja tima {TeamId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom ažuriranja tima" });
            }
        }

        /// <summary>
        /// Soft delete tima
        /// DELETE: api/team/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // TODO: Kada dodaš autentifikaciju, uzmi stvarni ID korisnika
                int deletedBy = 1; // Hardcoded za sada

                await _teamService.DeleteTeamAsync(id, deletedBy);

                return Ok(new { message = "Tim je uspešno obrisan" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (TeamLockedException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom brisanja tima {TeamId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom brisanja tima" });
            }
        }

        /// <summary>
        /// Vraća obrisani tim nazad (restore)
        /// PATCH: api/team/{id}/restore
        /// </summary>
        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _teamService.RestoreTeamAsync(id);

                return Ok(new { message = "Tim je uspešno vraćen" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom vraćanja tima {TeamId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom vraćanja tima" });
            }
        }

        /// <summary>
        /// Zaključava tim za izmene (lock)
        /// POST: api/team/{id}/lock
        /// </summary>
        [HttpPost("{id}/lock")]
        public async Task<IActionResult> Lock(int id)
        {
            try
            {
                // TODO: Kada dodaš autentifikaciju, uzmi stvarni ID korisnika
                int lockedBy = 1;

                await _teamService.LockTeamAsync(id, lockedBy);

                return Ok(new { message = "Tim je zaključan za izmene" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (TeamLockedException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom zaključavanja tima {TeamId}", id);
                return StatusCode(500, new { message = "Došlo je do greške" });
            }
        }

        /// <summary>
        /// Otključava tim (unlock)
        /// DELETE: api/team/{id}/lock
        /// </summary>
        [HttpDelete("{id}/lock")]
        public async Task<IActionResult> Unlock(int id)
        {
            try
            {
                // TODO: Kada dodaš autentifikaciju, uzmi stvarni ID korisnika
                int userId = 1;

                await _teamService.UnlockTeamAsync(id, userId);

                return Ok(new { message = "Tim je otključan" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom otključavanja tima {TeamId}", id);
                return StatusCode(500, new { message = "Došlo je do greške" });
            }
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync([FromQuery]  int leagueId, [FromQuery] int season)
        {
            try
            {
                await _teamService.SyncTeamsAsync(leagueId, season);
                return Ok(new { message = "Sinhronizacija timova uspešno završena. Proverite logove ili bazu za detalje." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom sinhronizacije timova");
                return StatusCode(500, new { message = "Došlo je do greške prilikom sinhronizacije", details = ex.ToString() });
            }
        }
    }
}
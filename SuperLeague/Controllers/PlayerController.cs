using Microsoft.AspNetCore.Mvc;
using SuperLeague.DTOs.Player;
using SuperLeague.Exceptions;
using SuperLeague.Interfaces.Service;

namespace SuperLeague.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly ILogger<PlayerController> _logger;

        public PlayerController(
            IPlayerService playerService,
            ILogger<PlayerController> logger)
        {
            _playerService = playerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var players = await _playerService.GetAllPlayersAsync();
                return Ok(players);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom preuzimanja svih igrača");
                return StatusCode(500, new { message = "Došlo je do greške prilikom preuzimanja igrača" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var player = await _playerService.GetPlayerByIdAsync(id);
                return Ok(player);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom preuzimanja igrača {PlayerId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom preuzimanja igrača" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlayerDto dto)
        {
            try
            {
                int createdBy = 1;

                var player = await _playerService.CreatePlayerAsync(dto, createdBy);

                return CreatedAtAction(
                    nameof(Get),
                    new { id = player.PlayerId },
                    new { message = "Igrač je uspešno kreiran", data = player }
                );
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom kreiranja igrača");
                return StatusCode(500, new { message = "Došlo je do greške prilikom kreiranja igrača" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerDto dto)
        {
            try
            {
                int updatedBy = 1;

                var player = await _playerService.UpdatePlayerAsync(id, dto, updatedBy);

                return Ok(new { message = "Igrač je uspešno ažuriran", data = player });
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
            catch (PlayerLockedException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom ažuriranja igrača {PlayerId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom ažuriranja igrača" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int deletedBy = 1;

                await _playerService.DeletePlayerAsync(id, deletedBy);

                return Ok(new { message = "Igrač je uspešno obrisan" });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessRuleException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (PlayerLockedException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Greška prilikom brisanja igrača {PlayerId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom brisanja igrača" });
            }
        }

        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                await _playerService.RestorePlayerAsync(id);

                return Ok(new { message = "Igrač je uspešno vraćen" });
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
                _logger.LogError(ex, "Greška prilikom vraćanja igrača {PlayerId}", id);
                return StatusCode(500, new { message = "Došlo je do greške prilikom vraćanja igrača" });
            }
        }

        
    }
}
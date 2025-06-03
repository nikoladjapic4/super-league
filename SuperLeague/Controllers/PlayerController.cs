using Microsoft.AspNetCore.Mvc;
using SuperLeague.Interfaces;


namespace SuperLeague.Controllers
{
    [ApiController]
    [Route("api/team")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerController(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        [HttpGet("{teamId}/players")]
        public async Task<IActionResult> GetAll(int teamId)
        {
            var players = await _playerRepository.GetAllAsync(teamId);
            return Ok(players);
        }


        [HttpGet("{teamId}/players/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var stats = await _playerRepository.GetStatsByIdAsync(id);
            return stats == null || !stats.Any() ? NotFound() : Ok(stats);
        }

        



    }
}

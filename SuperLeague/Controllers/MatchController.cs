using Microsoft.AspNetCore.Mvc;

namespace SuperLeague.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class MatchController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllMatches()
        {
            var matches = new[]
            {
                new {Id = 1, Host = "Red Star", Guest = "Partizan", HomeTeamScore = 1, AwayTeamScore = 2 },
                new {Id = 2, Host = "Partizan", Guest = "Red Star", HomeTeamScore = 2, AwayTeamScore = 2}
            };

            return Ok(matches);
        }

        [HttpGet("{matchId}")]
        public IActionResult GetMatch(int matchId)
        {
            var matches = new[]
            {
                new {Id = 1, Host = "Red Star", Guest = "Partizan", HomeTeamScore = 1, AwayTeamScore = 2 },
                new {Id = 2, Host = "Partizan", Guest = "Red Star", HomeTeamScore = 2, AwayTeamScore = 2}
            };

            var match = matches.FirstOrDefault(m => m.Id == matchId);

            if (match == null)
                return NotFound();

            return Ok(match);
        }

    }
}


using Microsoft.AspNetCore.Mvc;
using SuperLeague.Application.Services.Sync;

namespace SuperLeague.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataSyncController : ControllerBase
    {
        private readonly IDataSyncService _dataSyncService;
        private readonly ILogger<DataSyncController> _logger;

        public DataSyncController(IDataSyncService dataSyncService, ILogger<DataSyncController> logger)
        {
            _dataSyncService = dataSyncService;
            _logger = logger;
        }

        /// <summary>
        /// Sync data from Football API for Serbian SuperLiga
        /// POST: api/datasync/serbian-superliga
        /// </summary>
        /// 
        [HttpPost("Serbian-superleague")]
        public async Task<IActionResult> SyncSrbianSuperLeague([FromQuery] int season)
        {
            try
            {
                const int serbianLeagueId = 286;
                const int userId = 1;

                await _dataSyncService.SyncLeagueDataAsync(serbianLeagueId, season, userId);

                return Ok(new { message = $"Sync completed for season {season}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sync failed");
                return StatusCode(500, new {message = "Sync failed", error = ex.Message});
            }
        }

    }
}

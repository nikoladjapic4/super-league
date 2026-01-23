using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SuperLeague.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestConnectionController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TestConnectionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult TestConnection()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();

                var command = new SqlCommand("SELECT @@VERSION", connection);
                var version = command.ExecuteScalar()?.ToString();

                return Ok(new
                {
                    success = true,
                    message = "Konekcija uspešna!",
                    sqlVersion = version
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Greška u konekciji",
                    error = ex.Message,
                    connectionString = connectionString.Replace("Password=", "Password=***") // Sakrij password
                });
            }
        }
    }
}
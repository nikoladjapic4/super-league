
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SuperLeague.Application.Services;
using SuperLeague.Infrastructure.Repositories;
using SuperLeague.Tests.Fakes;

namespace SuperLeague.Tests
{
    public class TestContextFactory
    {
        public FakeClock Clock { get; private set; }
        public TeamService TeamService { get; private set; }
        public TeamRepository TeamRepository { get; private set; }

        private readonly IConfiguration _configuration;
        private readonly ILogger<TeamService> _logger;

        public TestContextFactory()
        {
            // Setup configuration
            var configBuilder = new ConfigurationBuilder()
                .AddUserSecrets<TestContextFactory>();
            _configuration = configBuilder.Build();

            // Setup logger
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            _logger = loggerFactory.CreateLogger<TeamService>();

            // Create clock
            Clock = new FakeClock();

            // Create repository
            TeamRepository = new TeamRepository(_configuration);

            // Create service
            TeamService = new TeamService(TeamRepository, _logger);
        }

        public static TestContextFactory Create() => new TestContextFactory();
    }
}
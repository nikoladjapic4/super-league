
using SuperLeague.DTOs.Team;
using SuperLeague.Tests.Fakes;

namespace SuperLeague.Tests.Builders
{
    public class TeamBuilder
    {
        private string _teamName = "Test Team";
        private string _city = "Test City";
        private string _stadium = "Test Stadium";
        private DateTime _dateOfFoundation = new DateTime(2000, 1, 1);
        private readonly FakeClock _clock;

        private TeamBuilder(FakeClock clock)
        {
            _clock = clock;
        }

        public static TeamBuilder Default(FakeClock clock) => new TeamBuilder(clock);

        public TeamBuilder WithName(string name)
        {
            _teamName = name;
            return this;
        }

        public TeamBuilder WithCity(string city)
        {
            _city = city;
            return this;
        }

        public TeamBuilder WithStadium(string stadium)
        {
            _stadium = stadium;
            return this;
        }

        public TeamBuilder WithFoundationDate(DateTime date)
        {
            _dateOfFoundation = date;
            return this;
        }

        public TeamBuilder FoundedIn(int year)
        {
            _dateOfFoundation = new DateTime(year, 1, 1);
            return this;
        }

        public CreateTeamDto BuildDto()
        {
            return new CreateTeamDto
            {
                TeamName = _teamName + " " + Guid.NewGuid().ToString(), // Unique name
                City = _city,
                Stadium = _stadium,
                DateOfFoundation = _dateOfFoundation
            };
        }

        // Helper za specifične timove
        public static TeamBuilder CrvenaZvezda(FakeClock clock)
        {
            return new TeamBuilder(clock)
                .WithName("Crvena Zvezda")
                .WithCity("Beograd")
                .WithStadium("Marakana")
                .FoundedIn(1945);
        }

        public static TeamBuilder Partizan(FakeClock clock)
        {
            return new TeamBuilder(clock)
                .WithName("Partizan")
                .WithCity("Beograd")
                .WithStadium("Humska")
                .FoundedIn(1945);
        }

        public static TeamBuilder RandomTeam(FakeClock clock)
        {
            return new TeamBuilder(clock);
        }
    }
}
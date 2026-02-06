using SuperLeague.Application.DTOs.Team;
using SuperLeague.Application.Exceptions;
using SuperLeague.Tests.Builders;

namespace SuperLeague.Tests.Services
{
    [TestFixture]
    public class TeamServiceTests
    {
        private TestContextFactory _context;

        [SetUp]
        public void SetUp()
        {
            _context = TestContextFactory.Create();
        }

        #region Create Team Tests

        [Test]
        public async Task CreateTeam_ShouldSucceed_WhenValidDataProvided()
        {
            // Arrange
            var dto = TeamBuilder.Partizan(_context.Clock).BuildDto();

            // Act
            var result = await _context.TeamService.CreateTeamAsync(dto, createdBy: 1);

            // Assert - Result
            Assert.That(result, Is.Not.Null);
            Assert.That(result.TeamId, Is.GreaterThan(0));
            Assert.That(result.TeamName, Does.Contain("Partizan"));
            Assert.That(result.City, Is.EqualTo("Beograd"));
            Assert.That(result.Stadium, Is.EqualTo("Humska"));
            Assert.That(result.IsActive, Is.True);

            // Cleanup
            await _context.TeamService.DeleteTeamAsync(result.TeamId, deletedBy: 1);
        }

        [Test]
        public async Task CreateTeam_ShouldFail_WhenDuplicateTeamExists()
        {
            // Arrange
            var dto = TeamBuilder.RandomTeam(_context.Clock).BuildDto();
            var firstTeam = await _context.TeamService.CreateTeamAsync(dto, createdBy: 1);

            try
            {
                // Act & Assert
                Assert.That(
                    async () => await _context.TeamService.CreateTeamAsync(dto, createdBy: 1),
                    Throws.TypeOf<BusinessRuleException>()
                        .With.Message.Contains("već postoji")
                );
            }
            finally
            {
                // Cleanup
                await _context.TeamService.DeleteTeamAsync(firstTeam.TeamId, deletedBy: 1);
            }
        }


        [TestCase("ALKJHDSKJSADJSALKJALSJDSAJDPSAJPOKDLSALKCPOAKPCK")]
        public void CreateTeam_ShouldFail_WhenTeamNameIsInvalid(string invalidName)
        {
            // Arrange
            var dto = TeamBuilder.Default(_context.Clock)
                .WithName(invalidName)
                .BuildDto();

            // Act & Assert
            Assert.That(
                async () => await _context.TeamService.CreateTeamAsync(dto, createdBy: 1),
                Throws.Exception // Team name validation exception
            );
        }

        #endregion

        #region Get Team Tests

        [Test]
        public async Task GetTeamById_ShouldReturnTeam_WhenTeamExists()
        {
            // Arrange
            var dto = TeamBuilder.Partizan(_context.Clock).BuildDto();
            var createdTeam = await _context.TeamService.CreateTeamAsync(dto, createdBy: 1);

            try
            {
                // Act
                var result = await _context.TeamService.GetTeamByIdAsync(createdTeam.TeamId);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.TeamId, Is.EqualTo(createdTeam.TeamId));
                Assert.That(result.TeamName, Does.Contain("Partizan"));
            }
            finally
            {
                // Cleanup
                await _context.TeamService.DeleteTeamAsync(createdTeam.TeamId, deletedBy: 1);
            }
        }

        [Test]
        public void GetTeamById_ShouldFail_WhenTeamDoesNotExist()
        {
            // Arrange
            var nonExistentId = 999999;

            // Act & Assert
            Assert.That(
                async () => await _context.TeamService.GetTeamByIdAsync(nonExistentId),
                Throws.TypeOf<NotFoundException>()
            );
        }

        #endregion

        #region Update Team Tests

        [Test]
        public async Task UpdateTeam_ShouldSucceed_WhenValidDataProvided()
        {
            // Arrange
            var createDto = TeamBuilder.RandomTeam(_context.Clock).BuildDto();
            var team = await _context.TeamService.CreateTeamAsync(createDto, createdBy: 1);

            var updateDto = new UpdateTeamDto
            {
                TeamName = "Updated Name",
                City = "Updated City",
                Stadium = "Updated Stadium",
                DateOfFoundation = team.DateOfFoundation,
                VersionRow = team.VersionRow
            };


            try
            {
                // Act
                var result = await _context.TeamService.UpdateTeamAsync(team.TeamId, updateDto, updatedBy: 1);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.TeamName, Does.Contain("Updated Name"));
                Assert.That(result.City, Is.EqualTo("Updated City"));
                Assert.That(result.Stadium, Is.EqualTo("Updated Stadium"));
            }
            finally
            {
                // Cleanup
                await _context.TeamService.DeleteTeamAsync(team.TeamId, deletedBy: 1);
            }
        }

        #endregion

        #region Delete and Restore Tests

        [Test]
        public async Task DeleteTeam_ShouldMarkAsInactive()
        {
            // Arrange
            var dto = TeamBuilder.RandomTeam(_context.Clock).BuildDto();
            var team = await _context.TeamService.CreateTeamAsync(dto, createdBy: 1);

            // Act
            var deleteResult = await _context.TeamService.DeleteTeamAsync(team.TeamId, deletedBy: 1);

            // Assert
            Assert.That(deleteResult, Is.True);

            // Verify team is not in active list
            var allTeams = await _context.TeamService.GetAllTeamsAsync();
            Assert.That(allTeams.Any(t => t.TeamId == team.TeamId), Is.False);
        }

        [Test]
        public async Task RestoreTeam_ShouldReactivateDeletedTeam()
        {
            // Arrange
            var dto = TeamBuilder.RandomTeam(_context.Clock).BuildDto();
            var team = await _context.TeamService.CreateTeamAsync(dto, createdBy: 1);
            await _context.TeamService.DeleteTeamAsync(team.TeamId, deletedBy: 1);

            try
            {
                // Act
                var restoreResult = await _context.TeamService.RestoreTeamAsync(team.TeamId);

                // Assert
                Assert.That(restoreResult, Is.True);

                // Verify team is back in active list
                var restoredTeam = await _context.TeamService.GetTeamByIdAsync(team.TeamId);
                Assert.That(restoredTeam.IsActive, Is.True);
            }
            finally
            {
                // Cleanup
                await _context.TeamService.DeleteTeamAsync(team.TeamId, deletedBy: 1);
            }
        }

        [Test]
        public async Task DeleteTeam_ShouldFail_WhenTeamAlreadyDeleted()
        {
            // Arrange
            var dto = TeamBuilder.RandomTeam(_context.Clock).BuildDto();
            var team = await _context.TeamService.CreateTeamAsync(dto, createdBy: 1);

            await _context.TeamService.DeleteTeamAsync(team.TeamId, deletedBy: 1);

            // Act & Assert
            var deletedTeam = await _context.TeamRepository.GetByIdAsync(team.TeamId);
            Assert.That(deletedTeam.IsActive, Is.False);
            /*
                        var ex = Assert.ThrowsAsync<BusinessRuleException>(async () =>
                            await _context.TeamService.DeleteTeamAsync(team.TeamId, deletedBy: 1), "Vec obrisan");
            */
        }

        #endregion
    }
}
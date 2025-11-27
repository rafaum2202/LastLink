using LastLink.Domain.Entities;
using LastLink.Domain.Enums;
using LastLink.Infra.Data;
using LastLink.Infra.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LastLink.Tests.Infra.Repositories
{
    public class AnticipationRepositoryTests
    {
        private LastLinkDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<LastLinkDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new LastLinkDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddAndReturnEntity_WhenSuccess()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            var dto = new Anticipation(Guid.NewGuid(), "abc", 200, 190, AnticipationStatusEnum.Pendente);

            var result = await repo.AddAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(1, db.AnticipationRequests.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            var dto = new Anticipation(Guid.NewGuid(), "abc", 200, 190, AnticipationStatusEnum.Pendente);
            db.AnticipationRequests.Add(dto);
            await db.SaveChangesAsync();

            var result = await repo.GetByIdAsync(dto.Id);

            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            var result = await repo.GetByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByCreatorAsync_ShouldReturnItems_WhenExists()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            db.AnticipationRequests.AddRange(
                new Anticipation(Guid.NewGuid(), "creator1", 200, 190, AnticipationStatusEnum.Pendente),
                new Anticipation(Guid.NewGuid(), "creator1", 300, 285, AnticipationStatusEnum.Aprovada),
                new Anticipation(Guid.NewGuid(), "creator2", 1000, 950, AnticipationStatusEnum.Pendente)
            );

            await db.SaveChangesAsync();

            var result = (await repo.GetByCreatorAsync("creator1")).ToList();

            Assert.Equal(2, result.Count);
            Assert.True(result.First().DataSolicitacao >= result.Last().DataSolicitacao);
        }

        [Fact]
        public async Task GetByCreatorAsync_ShouldReturnEmptyList_WhenNoItems()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            var result = await repo.GetByCreatorAsync("not_found");

            Assert.Empty(result);
        }

        [Fact]
        public async Task HasPendingForCreatorAsync_ShouldReturnTrue_WhenPendingExists()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            db.AnticipationRequests.Add(
                new Anticipation(Guid.NewGuid(), "creator1", 200, 190, AnticipationStatusEnum.Pendente)
            );

            await db.SaveChangesAsync();

            var result = await repo.HasPendingForCreatorAsync("creator1");

            Assert.True(result);
        }

        [Fact]
        public async Task HasPendingForCreatorAsync_ShouldReturnFalse_WhenNoPending()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            db.AnticipationRequests.Add(
                new Anticipation(Guid.NewGuid(), "creator1", 200, 190, AnticipationStatusEnum.Aprovada)
            );

            await db.SaveChangesAsync();

            var result = await repo.HasPendingForCreatorAsync("creator1");

            Assert.False(result);
        }

        [Fact]
        public async Task HasPendingForCreatorAsync_ShouldReturnFalse_WhenCreatorDoesNotExist()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            var result = await repo.HasPendingForCreatorAsync("ghost");

            Assert.False(result);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldPersistChanges_WhenCalled()
        {
            var db = CreateDbContext();
            var repo = new AnticipationRepository(db);

            var dto = new Anticipation(Guid.NewGuid(), "x", 100, 95, AnticipationStatusEnum.Pendente);

            db.AnticipationRequests.Add(dto);

            await repo.SaveChangesAsync();

            Assert.Equal(1, db.AnticipationRequests.Count());
        }
    }
}

using LastLink.Domain.Contracts.Repositories;
using LastLink.Domain.Enums;
using LastLink.Domain.Models.Dtos;
using LastLink.Infra.Data;
using Microsoft.EntityFrameworkCore;

namespace LastLink.Infra.Repositories
{
    public class AnticipationRepository : IAnticipationRepository
    {
        private readonly LastLinkDbContext _db;

        public AnticipationRepository(LastLinkDbContext db)
        {
            _db = db;
        }

        public async Task<AnticipationDto> AddAsync(AnticipationDto request)
        {
            await _db.AnticipationRequests.AddAsync(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public async Task<AnticipationDto?> GetByIdAsync(Guid id)
            => await _db.AnticipationRequests.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<IEnumerable<AnticipationDto>> GetByCreatorAsync(string creatorId)
            => await _db.AnticipationRequests.Where(x => x.CreatorId == creatorId)
                                             .OrderByDescending(x => x.DataSolicitacao)
                                             .ToListAsync();

        public async Task<bool> HasPendingForCreatorAsync(string creatorId)
            => await _db.AnticipationRequests.AnyAsync(x => x.CreatorId == creatorId && x.Status == AntecipationStatusEnum.Pendente);

        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();
    }
}

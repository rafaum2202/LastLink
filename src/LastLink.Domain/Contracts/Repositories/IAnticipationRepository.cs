using LastLink.Domain.Entities;

namespace LastLink.Domain.Contracts.Repositories
{
    public interface IAnticipationRepository
    {
        Task<Anticipation> AddAsync(Anticipation request);

        Task<IEnumerable<Anticipation>> GetByCreatorAsync(string creatorId);

        Task<Anticipation?> GetByIdAsync(Guid id);

        Task SaveChangesAsync();

        Task<bool> HasPendingForCreatorAsync(string creatorId);
    }
}
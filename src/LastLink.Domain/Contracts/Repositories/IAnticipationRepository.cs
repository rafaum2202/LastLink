using LastLink.Domain.Models.Dtos;

namespace LastLink.Domain.Contracts.Repositories
{
    public interface IAnticipationRepository
    {
        Task<AnticipationDto> AddAsync(AnticipationDto request);
        Task<IEnumerable<AnticipationDto>> GetByCreatorAsync(string creatorId);
        Task<AnticipationDto?> GetByIdAsync(Guid id);
        Task SaveChangesAsync();
        Task<bool> HasPendingForCreatorAsync(string creatorId);
    }
}

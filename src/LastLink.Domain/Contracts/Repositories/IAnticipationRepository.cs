using FluentResults;
using LastLink.Domain.Models.Dtos;
using LastLink.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

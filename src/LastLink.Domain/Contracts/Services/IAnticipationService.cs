using FluentResults;
using LastLink.Domain.Enums;
using LastLink.Domain.Models.Dtos;
using LastLink.Domain.Models.Requests;

namespace LastLink.Domain.Contracts.Services
{
    public interface IAnticipationService
    {
        Task<Result<AnticipationDto>> CreateAsync(CreateAnticipationRequest request);
        Task<Result<IEnumerable<AnticipationDto>>> ListByCreatorAsync(string creatorId);
        Result<AnticipationDto> Simulate(decimal valorSolicitado, string creatorId);
        Task<Result<AnticipationDto>> UpdateStatusAsync(Guid id, AnticipationStatusEnum newStatus);
    }
}

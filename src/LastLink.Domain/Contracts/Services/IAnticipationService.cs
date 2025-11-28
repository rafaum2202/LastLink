using FluentResults;
using LastLink.Domain.Models.Requests;
using LastLink.Domain.Models.Responses;

namespace LastLink.Domain.Contracts.Services
{
    public interface IAnticipationService
    {
        Task<Result<AnticipationResponse>> CreateAsync(CreateAnticipationRequest request);

        Task<Result<List<AnticipationResponse>>> ListByCreatorAsync(string creatorId);

        Result<AnticipationResponse> Simulate(SimulateRequest request);

        Task<Result<AnticipationResponse>> UpdateStatusAsync(Guid id, UpdateStatusRequest request);
    }
}
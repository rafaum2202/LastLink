using FluentResults;
using LastLink.Domain.Contracts.Repositories;
using LastLink.Domain.Contracts.Services;
using LastLink.Domain.Enums;
using LastLink.Domain.Errors;
using LastLink.Domain.Models.Requests;
using LastLink.Domain.Models.Responses;
using LastLink.Domain.Utils;

namespace LastLink.Application.Services
{
    public class AnticipationService : IAnticipationService
    {
        private const decimal MIN_VALUE = 100m;
        private const decimal TAX_RATE = 0.05m;
        private readonly IAnticipationRepository _repo;

        public AnticipationService(IAnticipationRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<AnticipationResponse>> CreateAsync(CreateAnticipationRequest request)
        {
            if (request.ValorSolicitado <= MIN_VALUE)
                return Result.Fail(ErrorMessages.VALOR_SOLICITADO_INVALIDO);

            if (await _repo.HasPendingForCreatorAsync(request.CreatorId))
                return Result.Fail(ErrorMessages.CREATOR_COM_SOLICITACAO_PENDENTE);

            var resultAdd = await _repo.AddAsync(request);

            if (resultAdd == null)
                return Result.Fail(ErrorMessages.ERRO_AO_CRIAR_SOLICITACAO);

            return Result.Ok((AnticipationResponse)resultAdd);
        }

        public async Task<Result<List<AnticipationResponse>>> ListByCreatorAsync(string creatorId)
        {
            var items = await _repo.GetByCreatorAsync(creatorId);

            if (items == null || !items.Any())
                return Result.Fail(ErrorMessages.CREATOR_SEM_SOLICITACOES);

            var response = items.Select(i => (AnticipationResponse)i).ToList();

            return Result.Ok(response);
        }

        public Result<AnticipationResponse> Simulate(string creatorId, decimal valorSolicitado)
        {
            if (valorSolicitado <= MIN_VALUE)
                return Result.Fail(ErrorMessages.VALOR_SOLICITADO_INVALIDO);

            return new AnticipationResponse(Guid.Empty, creatorId, valorSolicitado, Utils.CalculateValorLiquido(valorSolicitado, TAX_RATE), AnticipationStatusEnum.Simulação);
        }

        public async Task<Result<AnticipationResponse>> UpdateStatusAsync(Guid id, AnticipationStatusEnum newStatus)
        {
            var allowedStatuses = new HashSet<AnticipationStatusEnum>
            {
                AnticipationStatusEnum.Aprovada,
                AnticipationStatusEnum.Recusada
            };

            if (!allowedStatuses.Contains(newStatus))
                return Result.Fail(ErrorMessages.STATUS_INVALIDO);

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result.Fail(ErrorMessages.SOLICITACAO_NAO_ENCONTRADA);

            if (entity.Status != AnticipationStatusEnum.Pendente)
                return Result.Fail(ErrorMessages.SOLICITACAO_FINALIZADA);

            entity.Status = newStatus;
            await _repo.SaveChangesAsync();

            return Result.Ok((AnticipationResponse)entity);
        }
    }
}

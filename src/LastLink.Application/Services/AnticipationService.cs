using FluentResults;
using LastLink.Domain.Configurations;
using LastLink.Domain.Contracts.Repositories;
using LastLink.Domain.Contracts.Services;
using LastLink.Domain.Entities;
using LastLink.Domain.Enums;
using LastLink.Domain.Errors;
using LastLink.Domain.Models.Requests;
using LastLink.Domain.Models.Responses;
using LastLink.Domain.Utils;
using Microsoft.Extensions.Options;

namespace LastLink.Application.Services
{
    public class AnticipationService : IAnticipationService
    {
        private readonly IAnticipationRepository _repo;
        private readonly RulesConfig _rulesConfig;

        public AnticipationService(IAnticipationRepository repo, IOptions<RulesConfig> rulesConfig)
        {
            _repo = repo;
            _rulesConfig = rulesConfig.Value;
        }

        public async Task<Result<AnticipationResponse>> CreateAsync(CreateAnticipationRequest request)
        {
            if (await _repo.HasPendingForCreatorAsync(request.CreatorId))
                return Result.Fail(ErrorMessages.CREATOR_COM_SOLICITACAO_PENDENTE);

            var entity = new Anticipation(
                Guid.NewGuid(),
                request.CreatorId,
                request.ValorSolicitado,
                Utils.CalculateValorLiquido(request.ValorSolicitado, _rulesConfig.TaxRate),
                AnticipationStatusEnum.Pendente
            );

            var resultAdd = await _repo.AddAsync(entity);

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

        public Result<AnticipationResponse> Simulate(SimulateRequest request)
        {
            return new AnticipationResponse(Guid.Empty, request.CreatorId, request.ValorSolicitado, Utils.CalculateValorLiquido(request.ValorSolicitado, _rulesConfig.TaxRate), AnticipationStatusEnum.Simulação);
        }

        public async Task<Result<AnticipationResponse>> UpdateStatusAsync(Guid id, UpdateStatusRequest request)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result.Fail(ErrorMessages.SOLICITACAO_NAO_ENCONTRADA);

            if (entity.Status != AnticipationStatusEnum.Pendente)
                return Result.Fail(ErrorMessages.SOLICITACAO_FINALIZADA);

            entity.Status = request.Status;
            await _repo.SaveChangesAsync();

            return Result.Ok((AnticipationResponse)entity);
        }
    }
}
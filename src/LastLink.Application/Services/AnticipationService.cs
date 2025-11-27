using FluentResults;
using LastLink.Domain.Contracts.Repositories;
using LastLink.Domain.Contracts.Services;
using LastLink.Domain.Enums;
using LastLink.Domain.Errors;
using LastLink.Domain.Models.Dtos;
using LastLink.Domain.Models.Requests;

namespace LastLink.Application.Services
{
    public class AnticipationService : IAnticipationService
    {
        private const decimal TAX_RATE = 0.05m;
        private readonly IAnticipationRepository _repo;

        public AnticipationService(IAnticipationRepository repo)
        {
            _repo = repo;
        }

        public async Task<Result<AnticipationDto>> CreateAsync(CreateAnticipationRequest request)
        {
            if (request.ValorSolicitado <= 100m)
                return Result.Fail(ErrorMessages.VALOR_SOLICITADO_INVALIDO);

            if (await _repo.HasPendingForCreatorAsync(request.CreatorId))
                return Result.Fail(ErrorMessages.CREATOR_COM_SOLICITACAO_PENDENTE);

            var anticipationDto = new AnticipationDto
            {
                CreatorId = request.CreatorId,
                ValorSolicitado = request.ValorSolicitado,
                ValorLiquido = CalculateValorLiquido(request.ValorSolicitado),
                DataSolicitacao = DateTime.Now,
                Status = AntecipationStatusEnum.Pendente
            };

            var resultAdd = await _repo.AddAsync(anticipationDto);

            if (resultAdd == null)
                return Result.Fail(ErrorMessages.ERRO_AO_CRIAR_SOLICITACAO);

            return Result.Ok(resultAdd);
        }

        public async Task<Result<IEnumerable<AnticipationDto>>> ListByCreatorAsync(string creatorId)
        {
            var items = await _repo.GetByCreatorAsync(creatorId);

            if (items == null || !items.Any())
                return Result.Fail(ErrorMessages.CREATOR_SEM_SOLICITACOES);

            return Result.Ok(items);
        }

        public Result<AnticipationDto> Simulate(decimal valorSolicitado, string creatorId)
        {
            if (valorSolicitado <= 100m)
                return Result.Fail(ErrorMessages.VALOR_SOLICITADO_INVALIDO);

            return new AnticipationDto
            {
                Id = Guid.Empty,
                CreatorId = creatorId,
                ValorSolicitado = valorSolicitado,
                ValorLiquido = CalculateValorLiquido(valorSolicitado),
                DataSolicitacao = DateTime.Now,
                Status = AntecipationStatusEnum.Simulação
            };
        }

        public async Task<Result<AnticipationDto>> UpdateStatusAsync(Guid id, AntecipationStatusEnum newStatus)
        {
            var allowedStatuses = new HashSet<AntecipationStatusEnum>
            {
                AntecipationStatusEnum.Aprovada,
                AntecipationStatusEnum.Recusada
            };

            if (!allowedStatuses.Contains(newStatus))
                return Result.Fail(ErrorMessages.VALOR_SOLICITADO_INVALIDO);

            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return Result.Fail(ErrorMessages.CREATOR_SEM_SOLICITACOES);

            entity.Status = newStatus;
            await _repo.SaveChangesAsync();

            return Result.Ok(entity);
        }

        private static decimal CalculateValorLiquido(decimal bruto)
            => Math.Round(bruto * (1 - TAX_RATE), 2, MidpointRounding.AwayFromZero);
    }
}

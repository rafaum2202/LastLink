using LastLink.Domain.Models.Dtos;

namespace LastLink.Domain.Models.Requests
{
    public class CreateAnticipationRequest
    {
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }

        private readonly decimal taxRate = 0.05m;

        public static implicit operator AnticipationDto(CreateAnticipationRequest request) =>
            new AnticipationDto(Guid.NewGuid(), request.CreatorId, request.ValorSolicitado, Utils.Utils.CalculateValorLiquido(request.ValorSolicitado, request.taxRate), Enums.AnticipationStatusEnum.Pendente);
    }
}

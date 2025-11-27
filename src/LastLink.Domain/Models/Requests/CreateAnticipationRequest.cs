using LastLink.Domain.Entities;

namespace LastLink.Domain.Models.Requests
{
    public class CreateAnticipationRequest
    {
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }

        private readonly decimal taxRate = 0.05m;

        public static implicit operator Anticipation(CreateAnticipationRequest request) =>
            new Anticipation(Guid.NewGuid(), request.CreatorId, request.ValorSolicitado, Utils.Utils.CalculateValorLiquido(request.ValorSolicitado, request.taxRate), Enums.AnticipationStatusEnum.Pendente);
    }
}

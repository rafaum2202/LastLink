using LastLink.Domain.Entities;
using LastLink.Domain.Enums;

namespace LastLink.Domain.Models.Responses
{
    public class AnticipationResponse
    {
        public Guid Id { get; set; }
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }
        public decimal ValorLiquido { get; set; }
        public AnticipationStatusEnum Status { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public AnticipationResponse()
        {
        }

        public AnticipationResponse(Guid id, string creatorId, decimal valorSolicitado, decimal valorLiquido, AnticipationStatusEnum status)
        {
            Id = id;
            CreatorId = creatorId;
            ValorSolicitado = valorSolicitado;
            ValorLiquido = valorLiquido;
            Status = status;
            DataSolicitacao = DateTime.Now;
        }

        public static implicit operator AnticipationResponse(Anticipation a) => new()
        {
            Id = a.Id,
            CreatorId = a.CreatorId,
            ValorSolicitado = a.ValorSolicitado,
            ValorLiquido = a.ValorLiquido,
            Status = a.Status,
            DataSolicitacao = a.DataSolicitacao
        };



    }
}

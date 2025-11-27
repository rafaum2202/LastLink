using LastLink.Domain.Enums;

namespace LastLink.Domain.Models.Dtos
{
    public class AnticipationDto
    {
        public Guid Id { get; set; }
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }
        public decimal ValorLiquido { get; set; }
        public AnticipationStatusEnum Status { get; set; }
        public DateTime DataSolicitacao { get; set; }

        public AnticipationDto(Guid id, string creatorId, decimal valorSolicitado, decimal valorLiquido, AnticipationStatusEnum status)
        {
            Id = id;
            CreatorId = creatorId;
            ValorSolicitado = valorSolicitado;
            ValorLiquido = valorLiquido;
            Status = status;
            DataSolicitacao = DateTime.Now;
        }
    }
}

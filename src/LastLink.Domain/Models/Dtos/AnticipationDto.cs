using LastLink.Domain.Enums;

namespace LastLink.Domain.Models.Dtos
{
    public class AnticipationDto
    {
        public Guid Id { get; set; }
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }
        public decimal ValorLiquido { get; set; }
        public DateTime DataSolicitacao { get; set; }
        public AntecipationStatusEnum Status { get; set; }
    }
}

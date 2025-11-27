namespace LastLink.Domain.Models.Requests
{
    public class CreateAnticipationRequest
    {
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }
    }
}

namespace LastLink.Domain.Models.Requests
{
    public class SimulateRequest
    {
        public string CreatorId { get; set; } = null!;
        public decimal ValorSolicitado { get; set; }
    }
}
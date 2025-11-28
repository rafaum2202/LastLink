using LastLink.Domain.Enums;

namespace LastLink.Domain.Models.Requests
{
    public class UpdateStatusRequest
    {
        public AnticipationStatusEnum Status { get; set; }
    }
}
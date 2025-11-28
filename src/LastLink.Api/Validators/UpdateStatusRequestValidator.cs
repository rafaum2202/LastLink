using FluentValidation;
using LastLink.Domain.Enums;
using LastLink.Domain.Models.Requests;

namespace LastLink.Api.Validators
{
    public class UpdateStatusRequestValidator : AbstractValidator<UpdateStatusRequest>
    {
        public UpdateStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                        .Must(status =>
                            status == AnticipationStatusEnum.Aprovada ||
                            status == AnticipationStatusEnum.Recusada);
        }
    }
}
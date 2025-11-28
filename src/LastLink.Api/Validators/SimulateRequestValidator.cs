using FluentValidation;
using LastLink.Domain.Configurations;
using LastLink.Domain.Models.Requests;
using Microsoft.Extensions.Options;

namespace LastLink.Api.Validators
{
    public class SimulateRequestValidator : AbstractValidator<SimulateRequest>
    {
        public SimulateRequestValidator(IOptions<RulesConfig> rulesConfig)
        {
            var rules = rulesConfig.Value;

            RuleFor(x => x.CreatorId)
                .NotEmpty();

            RuleFor(x => x.ValorSolicitado)
                .GreaterThan(rules.MinValueAllowed);
        }
    }
}
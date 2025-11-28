using FluentValidation.TestHelper;
using LastLink.Api.Validators;
using LastLink.Domain.Configurations;
using LastLink.Domain.Models.Requests;
using Microsoft.Extensions.Options;

namespace LastLink.Tests.Api.Validators
{
    public class CreateAnticipationRequestValidatorTests
    {
        private readonly CreateAnticipationRequestValidator _validator;

        public CreateAnticipationRequestValidatorTests()
        {
            var rules = Options.Create(new RulesConfig { MinValueAllowed = 100 });
            _validator = new CreateAnticipationRequestValidator(rules);
        }

        [Fact]
        public void Should_Have_Error_When_CreatorId_Is_Empty()
        {
            var model = new CreateAnticipationRequest
            {
                CreatorId = "",
                ValorSolicitado = 200
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.CreatorId);
        }

        [Fact]
        public void Should_Have_Error_When_ValorSolicitado_Is_Less_Than_MinValue()
        {
            var model = new CreateAnticipationRequest
            {
                CreatorId = "abc",
                ValorSolicitado = 50
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ValorSolicitado);
        }

        [Fact]
        public void Should_Pass_When_Data_Is_Valid()
        {
            var model = new CreateAnticipationRequest
            {
                CreatorId = "abc",
                ValorSolicitado = 150
            };

            var result = _validator.TestValidate(model);
            Assert.True(result.IsValid);
        }
    }
}
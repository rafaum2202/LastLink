using FluentValidation.TestHelper;
using LastLink.Api.Validators;
using LastLink.Domain.Configurations;
using LastLink.Domain.Models.Requests;
using Microsoft.Extensions.Options;

namespace LastLink.Tests.Api.Validators
{
    public class SimulateRequestValidatorTests
    {
        private readonly SimulateRequestValidator _validator;

        public SimulateRequestValidatorTests()
        {
            var rules = Options.Create(new RulesConfig { MinValueAllowed = 100 });
            _validator = new SimulateRequestValidator(rules);
        }

        [Fact]
        public void Should_Have_Error_When_CreatorId_Is_Empty()
        {
            var model = new SimulateRequest
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
            var model = new SimulateRequest
            {
                CreatorId = "abc",
                ValorSolicitado = 30
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.ValorSolicitado);
        }

        [Fact]
        public void Should_Pass_When_Data_Is_Valid()
        {
            var model = new SimulateRequest
            {
                CreatorId = "abc",
                ValorSolicitado = 1000
            };

            var result = _validator.TestValidate(model);
            Assert.True(result.IsValid);
        }
    }
}
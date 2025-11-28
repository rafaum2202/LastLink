using FluentValidation.TestHelper;
using LastLink.Api.Validators;
using LastLink.Domain.Enums;
using LastLink.Domain.Models.Requests;

namespace LastLink.Tests.Api.Validators
{
    public class UpdateStatusRequestValidatorTests
    {
        private readonly UpdateStatusRequestValidator _validator;

        public UpdateStatusRequestValidatorTests()
        {
            _validator = new UpdateStatusRequestValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Status_Is_Invalid()
        {
            var model = new UpdateStatusRequest
            {
                Status = (AnticipationStatusEnum)999
            };

            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Pass_When_Status_Is_Aprovada()
        {
            var model = new UpdateStatusRequest
            {
                Status = AnticipationStatusEnum.Aprovada
            };

            var result = _validator.TestValidate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Should_Pass_When_Status_Is_Recusada()
        {
            var model = new UpdateStatusRequest
            {
                Status = AnticipationStatusEnum.Recusada
            };

            var result = _validator.TestValidate(model);
            Assert.True(result.IsValid);
        }
    }
}
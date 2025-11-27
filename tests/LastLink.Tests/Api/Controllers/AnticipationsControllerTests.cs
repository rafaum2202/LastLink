using FluentResults;
using LastLink.Api.Controllers.V1;
using LastLink.Domain.Contracts.Services;
using LastLink.Domain.Enums;
using LastLink.Domain.Errors;
using LastLink.Domain.Models.Dtos;
using LastLink.Domain.Models.Requests;
using LastLink.Domain.Utils;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LastLink.Tests.Api
{
    public class AnticipationsControllerTests
    {
        private const decimal TAX_RATE = 0.05m;
        private readonly Mock<IAnticipationService> _serviceMock;
        private readonly AnticipationsController _controller;

        public AnticipationsControllerTests()
        {
            _serviceMock = new Mock<IAnticipationService>();
            _controller = new AnticipationsController(_serviceMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnCreated_WhenSuccess()
        {
            var request = new CreateAnticipationRequest
            {
                CreatorId = "123",
                ValorSolicitado = 200m
            };

            var dto = new AnticipationDto(Guid.NewGuid(), "123", 200m, Utils.CalculateValorLiquido(200m, TAX_RATE), AnticipationStatusEnum.Pendente);

            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateAnticipationRequest>()))
                        .ReturnsAsync(Result.Ok(dto));

            var result = await _controller.Create(request);

            Assert.IsType<CreatedResult>(result);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnBadRequest_WhenFail()
        {
            var request = new CreateAnticipationRequest
            {
                CreatorId = "123",
                ValorSolicitado = 10m
            };

            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateAnticipationRequest>()))
                        .ReturnsAsync(Result.Fail(ErrorMessages.VALOR_SOLICITADO_INVALIDO));

            var result = await _controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ListByCreatorAsync_ShouldReturnOk_WhenSuccess()
        {
            var creatorId = "123";

            var list = new List<AnticipationDto>
            {
                new AnticipationDto(Guid.NewGuid(), creatorId, 200m, 190m, AnticipationStatusEnum.Pendente)
            };

            _serviceMock.Setup(s => s.ListByCreatorAsync(It.IsAny<string>()))
                        .ReturnsAsync(Result.Ok<IEnumerable<AnticipationDto>>(list));

            var result = await _controller.ListByCreator(creatorId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ListByCreatorAsync_ShouldReturnNotFound_WhenFail()
        {
            _serviceMock.Setup(s => s.ListByCreatorAsync(It.IsAny<string>()))
                        .ReturnsAsync(Result.Fail<IEnumerable<AnticipationDto>>(ErrorMessages.CREATOR_SEM_SOLICITACOES));

            var result = await _controller.ListByCreator("123");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Simulate_ShouldReturnCreated_WhenSuccess()
        {
            var creatorId = "123";
            var valor = 200m;

            var dto = new AnticipationDto(Guid.Empty, creatorId, valor, 190m, AnticipationStatusEnum.Simulação);

            _serviceMock.Setup(s => s.Simulate(It.IsAny<string>(), It.IsAny<decimal>()))
                        .Returns(Result.Ok(dto));

            var result = _controller.Simulate(creatorId, valor);

            var created = Assert.IsType<CreatedResult>(result);
            var returnedDto = Assert.IsType<AnticipationDto>(created.Value);

            Assert.Equal(dto.CreatorId, returnedDto.CreatorId);
            Assert.Equal(201, created.StatusCode);
        }

        [Fact]
        public void Simulate_ShouldReturnBadRequest_WhenFail()
        {
            var creatorId = "123";
            var valor = 50m;

            _serviceMock.Setup(s => s.Simulate(It.IsAny<string>(), It.IsAny<decimal>()))
                        .Returns(Result.Fail<AnticipationDto>(ErrorMessages.VALOR_SOLICITADO_INVALIDO));

            var result = _controller.Simulate(creatorId, valor);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnOk_WhenSuccess()
        {
            var id = Guid.NewGuid();
            var status = AnticipationStatusEnum.Aprovada;

            var dto = new AnticipationDto(id, "123", 1000m, 950m, status);

            _serviceMock.Setup(s => s.UpdateStatusAsync(id, status))
                        .ReturnsAsync(Result.Ok(dto));

            var result = await _controller.UpdateStatus(id, status);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(dto, ok.Value);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnBadRequest_WhenFail()
        {
            _serviceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<AnticipationStatusEnum>()))
                        .ReturnsAsync(Result.Fail<AnticipationDto>(ErrorMessages.STATUS_INVALIDO));

            var result = await _controller.UpdateStatus(Guid.NewGuid(), AnticipationStatusEnum.Recusada);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}

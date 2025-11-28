using FluentResults;
using LastLink.Api.Controllers.V1;
using LastLink.Domain.Contracts.Services;
using LastLink.Domain.Enums;
using LastLink.Domain.Errors;
using LastLink.Domain.Models.Requests;
using LastLink.Domain.Models.Responses;
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

            var response = new AnticipationResponse(Guid.NewGuid(), "123", 200m, Utils.CalculateValorLiquido(200m, TAX_RATE), AnticipationStatusEnum.Pendente);

            _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateAnticipationRequest>()))
                        .ReturnsAsync(Result.Ok(response));

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
                        .ReturnsAsync(Result.Fail(ErrorMessages.CREATOR_COM_SOLICITACAO_PENDENTE));

            var result = await _controller.Create(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ListByCreatorAsync_ShouldReturnOk_WhenSuccess()
        {
            var creatorId = "123";

            var list = new List<AnticipationResponse>
            {
                new AnticipationResponse(Guid.NewGuid(), creatorId, 200m, 190m, AnticipationStatusEnum.Pendente)
            };

            _serviceMock.Setup(s => s.ListByCreatorAsync(It.IsAny<string>()))
                        .ReturnsAsync(Result.Ok(list));

            var result = await _controller.ListByCreator(creatorId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ListByCreatorAsync_ShouldReturnNotFound_WhenFail()
        {
            _serviceMock.Setup(s => s.ListByCreatorAsync(It.IsAny<string>()))
                        .ReturnsAsync(Result.Fail<List<AnticipationResponse>>(ErrorMessages.CREATOR_SEM_SOLICITACOES));

            var result = await _controller.ListByCreator("123");

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Simulate_ShouldReturnCreated_WhenSuccess()
        {
            var creatorId = "123";
            var valor = 200m;

            var request = new SimulateRequest();
            var response = new AnticipationResponse(Guid.Empty, creatorId, valor, 190m, AnticipationStatusEnum.Simulação);

            _serviceMock.Setup(s => s.Simulate(It.IsAny<SimulateRequest>()))
                        .Returns(Result.Ok(response));

            var result = _controller.Simulate(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public void Simulate_ShouldReturnBadRequest_WhenFail()
        {
            var creatorId = "123";
            var valor = 50m;

            var request = new SimulateRequest();
            var response = new AnticipationResponse(Guid.Empty, creatorId, valor, 190m, AnticipationStatusEnum.Simulação);

            _serviceMock.Setup(s => s.Simulate(It.IsAny<SimulateRequest>()))
                        .Returns(Result.Fail<AnticipationResponse>(ErrorMessages.ERRO_AO_CRIAR_SOLICITACAO));

            var result = _controller.Simulate(request);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnOk_WhenSuccess()
        {
            var id = Guid.NewGuid();
            var request = new UpdateStatusRequest();

            var response = new AnticipationResponse(id, "123", 1000m, 950m, AnticipationStatusEnum.Aprovada);

            _serviceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateStatusRequest>()))
                        .ReturnsAsync(Result.Ok(response));

            var result = await _controller.UpdateStatus(id, request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, ok.Value);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnBadRequest_WhenFail()
        {
            var request = new UpdateStatusRequest();
            _serviceMock.Setup(s => s.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<UpdateStatusRequest>()))
                        .ReturnsAsync(Result.Fail<AnticipationResponse>(ErrorMessages.SOLICITACAO_FINALIZADA));

            var result = await _controller.UpdateStatus(Guid.NewGuid(), request);

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}
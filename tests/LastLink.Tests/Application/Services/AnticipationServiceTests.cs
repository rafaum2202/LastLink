using LastLink.Application.Services;
using LastLink.Domain.Contracts.Repositories;
using LastLink.Domain.Entities;
using LastLink.Domain.Enums;
using LastLink.Domain.Errors;
using LastLink.Domain.Models.Requests;
using Moq;

namespace LastLink.Tests.Application.Services
{
    public class AnticipationServiceTests
    {
        private readonly Mock<IAnticipationRepository> _repoMock;
        private readonly AnticipationService _service;

        public AnticipationServiceTests()
        {
            _repoMock = new Mock<IAnticipationRepository>();
            _service = new AnticipationService(_repoMock.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenValorSolicitadoIsInvalid()
        {
            var request = new CreateAnticipationRequest
            {
                CreatorId = "123",
                ValorSolicitado = 50m
            };

            var result = await _service.CreateAsync(request);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.VALOR_SOLICITADO_INVALIDO.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenCreatorHasPendingRequest()
        {
            var request = new CreateAnticipationRequest
            {
                CreatorId = "123",
                ValorSolicitado = 200m
            };

            _repoMock.Setup(r => r.HasPendingForCreatorAsync("123"))
                     .ReturnsAsync(true);

            var result = await _service.CreateAsync(request);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.CREATOR_COM_SOLICITACAO_PENDENTE.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnFail_WhenAddAsyncReturnsNull()
        {
            var request = new CreateAnticipationRequest
            {
                CreatorId = "123",
                ValorSolicitado = 200m
            };

            _repoMock.Setup(r => r.HasPendingForCreatorAsync("123"))
                     .ReturnsAsync(false);

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Anticipation>()))
                     .ReturnsAsync((Anticipation?)null);

            var result = await _service.CreateAsync(request);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.ERRO_AO_CRIAR_SOLICITACAO.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnSuccess_WhenRequestIsValid()
        {
            var request = new CreateAnticipationRequest
            {
                CreatorId = "123",
                ValorSolicitado = 200m
            };

            var created = (Anticipation)request;

            _repoMock.Setup(r => r.HasPendingForCreatorAsync("123"))
                     .ReturnsAsync(false);

            _repoMock.Setup(r => r.AddAsync(It.IsAny<Anticipation>()))
                     .ReturnsAsync(created);

            var result = await _service.CreateAsync(request);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task ListByCreatorAsync_ShouldReturnFail_WhenNoItemsFound()
        {
            _repoMock.Setup(r => r.GetByCreatorAsync("abc"))
                     .ReturnsAsync(new List<Anticipation>());

            var result = await _service.ListByCreatorAsync("abc");

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.CREATOR_SEM_SOLICITACOES.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task ListByCreatorAsync_ShouldReturnSuccess_WhenItemsExist()
        {
            var items = new List<Anticipation>
            {
                new Anticipation(Guid.NewGuid(), "abc", 1000m, 950m, AnticipationStatusEnum.Pendente)
            };

            _repoMock.Setup(r => r.GetByCreatorAsync("abc"))
                     .ReturnsAsync(items);

            var result = await _service.ListByCreatorAsync("abc");

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void Simulate_ShouldReturnFail_WhenValorSolicitadoIsInvalid()
        {
            var result = _service.Simulate("123", 50m);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.VALOR_SOLICITADO_INVALIDO.Message, result.Errors.First().Message);
        }

        [Fact]
        public void Simulate_ShouldReturnSuccess_WhenInputIsValid()
        {
            var result = _service.Simulate("123", 200m);

            Assert.True(result.IsSuccess);
            Assert.Equal("123", result.Value.CreatorId);
            Assert.Equal(200m, result.Value.ValorSolicitado);
            Assert.Equal(AnticipationStatusEnum.Simulação, result.Value.Status);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFail_WhenStatusIsInvalid()
        {
            var id = Guid.NewGuid();

            var result = await _service.UpdateStatusAsync(id, AnticipationStatusEnum.Simulação);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.STATUS_INVALIDO.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFail_WhenEntityNotFound()
        {
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync((Anticipation?)null);

            var result = await _service.UpdateStatusAsync(id, AnticipationStatusEnum.Aprovada);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.SOLICITACAO_NAO_ENCONTRADA.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnFail_WhenEntityNotPendente()
        {
            var id = Guid.NewGuid();
            var dto = new Anticipation(id, "123", 1000m, 950m, AnticipationStatusEnum.Aprovada);

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(dto);

            var result = await _service.UpdateStatusAsync(id, AnticipationStatusEnum.Recusada);

            Assert.True(result.IsFailed);
            Assert.Equal(ErrorMessages.SOLICITACAO_FINALIZADA.Message, result.Errors.First().Message);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldReturnSuccess_WhenUpdatedCorrectly()
        {
            var id = Guid.NewGuid();
            var dto = new Anticipation(id, "123", 1000m, 950m, AnticipationStatusEnum.Pendente);

            _repoMock.Setup(r => r.GetByIdAsync(id))
                     .ReturnsAsync(dto);

            var result = await _service.UpdateStatusAsync(id, AnticipationStatusEnum.Aprovada);

            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.True(result.IsSuccess);
            Assert.Equal(AnticipationStatusEnum.Aprovada, result.Value.Status);
        }
    }
}

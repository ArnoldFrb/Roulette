using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Roulette.Application.Models;
using Roulette.Application.RouletteServices;
using Roulette.Application.UserServices;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;
using System.Linq.Expressions;

namespace Roulette.Application.Test
{
    public class CloseRouletteTest
    {
        private readonly RouletteEntity _roulette;
        private readonly GamblerEntity _user;
        private readonly List<BetEntity> _bet;

        private readonly Mock<IRouletteRepository> _rouletteRepository;
        private readonly Mock<IGamblerRepository> _gamblerRepository;
        private readonly Mock<IBetRepository> _betRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRedisCacheService> _redis;

        private readonly Mock<ILogger<CloseRouletteService>> _loggerClose;
        private readonly Mock<ILogger<OpenRouletteService>> _loggerOpen;

        public CloseRouletteTest()
        {

            _rouletteRepository = new Mock<IRouletteRepository>();
            _gamblerRepository = new Mock<IGamblerRepository>();
            _betRepository = new Mock<IBetRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _redis = new Mock<IRedisCacheService>();

            _loggerClose = new Mock<ILogger<CloseRouletteService>>();
            _loggerOpen = new Mock<ILogger<OpenRouletteService>>();

            _user = new GamblerEntity("Jose Carlos", 50000) { Id = 1 };
            _roulette = new RouletteEntity() { Id = 1};

            _bet =
            [
                new(100, BetType.Color, RouletteColor.Red, null, _user.Id, _roulette.Id) { Id = 1 },
            ];
        }

        /*
         1.	Debe devolver error si la ruleta no existe
            •	Dado una una petición para cerrar una ruleta con Id 5 que no existe en la base de datos
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "Error closing roulette: Roulette not found."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_ShouldReturnNotFound_WhenRouletteDoesNotExist()
        {
            // Arrange
            var service = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await service.ExecuteAsync(5);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.Roulette.ROULETTE_NOT_FOUND);
            response.Message.Should().Be("Error closing roulette: Roulette not found.");
        }

        /*
         2.	Debe cerrar la ruleta correctamente
            •	Dado una una petición para cerrar una ruleta con Id 1 que existe en la base de datos
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id no null, Status "Closed", Date fecha actual de apertura y Message "Roulette opened successfully."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_ShouldReturnBets_WhenRouletteHasBets()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _betRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<BetEntity, bool>>>())).ReturnsAsync(_bet);

            var serviceO = new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object);
            await serviceO.ExecuteAsync(1);

            var serviceC = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await serviceC.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Code.Should().Be(AppCodes.Roulette.ROULETTE_CLOSED);
            response.Message.Should().Be("Roulette closed successfully.");
        }

        /*
         3.	Debe cerrar la ruleta correctamente
            •	Dado una una petición para cerrar una ruleta con Id 1 que existe en la base de datos y no tiene apuestas
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id no null, Status "Closed", Date fecha actual de apertura y Message "Roulette opened successfully."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_ShouldCloseRoulette_WhenNoBetsExist()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _betRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<BetEntity, bool>>>())).ReturnsAsync([]);

            var serviceO = new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object);
            await serviceO.ExecuteAsync(1);

            var serviceC = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await serviceC.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Code.Should().Be(AppCodes.Roulette.ROULETTE_CLOSED);
            response.Message.Should().Be("Roulette closed successfully. No bets found for this roulette.");
        }

        /*
         4.	Debe devolver error si FindSingleOrDefault lanza excepción
            •	Dado una una petición para cerrar una ruleta y FindSingleOrDefault lanza una excepción
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "DB error."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_WhenFindThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Throws(new Exception("DB error."));
            var service = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
            response.Message.Should().Be("Error closing roulette: DB error.");
        }

        /*
         5.	Debe devolver error si FindBy lanza excepción
            •	Dado una una petición para cerrar una ruleta y FindBy lanza una excepción
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "DB error."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_WhenFindByThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _betRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<BetEntity, bool>>>())).Throws(new Exception("Search error."));

            _roulette.OpenRoulette();

            var service = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
            response.Message.Should().Be("Error closing roulette: Search error.");
        }

        /*
         6.	Debe devolver error si la apuesta no se ha abierto y lanzar excepción
            •	Dado una una petición para cerrar una ruleta y esta no ha sido abierta
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "The roulette is not Open."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_ShouldReturnError_WhenRouletteIsNotOpen()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _betRepository.Setup(r => r.FindByAsync(It.IsAny<Expression<Func<BetEntity, bool>>>())).ReturnsAsync(_bet);
            var service = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.Roulette.ROULETTE_CLOSE_ERROR);
            response.Message.Should().Be("Error closing roulette: Expected status Open, but current is Created.");
        }

        /*
         7.	Excepción inesperada
            •	Dado una una petición para cerrar una ruleta y se produce una excepción inesperada
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "Error closing roulette: 'Db Error'"
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public async Task Execute_ShouldReturnErrorMessage_WhenExceptionOccurs()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Throws(new Exception("Db Error."));
            var service = new CloseRouletteService(_rouletteRepository.Object, _betRepository.Object, _gamblerRepository.Object, _unitOfWork.Object, _redis.Object, _loggerClose.Object);

            // Act
            var response = await service.ExecuteAsync(1);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
            response.Message.Should().Be("Error closing roulette: Db Error.");
        }
    }
}

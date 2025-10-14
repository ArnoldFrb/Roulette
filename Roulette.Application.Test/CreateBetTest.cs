using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Roulette.Application.BetServices;
using Roulette.Application.Models.Requests;
using Roulette.Application.RouletteServices;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;
using System.Linq.Expressions;

namespace Roulette.Application.Test
{
    public class CreateBetTest
    {
        private readonly RouletteEntity _roulette;
        private readonly GamblerEntity _user;
        private readonly BetEntity _bet;

        private readonly Mock<IRouletteRepository> _rouletteRepository;
        private readonly Mock<IGamblerRepository> _gamblerRepository;
        private readonly Mock<IBetRepository> _betRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRedisCacheService> _redis;

        private readonly Mock<ILogger<CreateBetService>> _loggerBet;
        private readonly Mock<ILogger<OpenRouletteService>> _loggerOpen;

        public CreateBetTest()
        {

            _rouletteRepository = new Mock<IRouletteRepository>();
            _gamblerRepository = new Mock<IGamblerRepository>();
            _betRepository = new Mock<IBetRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _redis = new Mock<IRedisCacheService>();

            _loggerBet = new Mock<ILogger<CreateBetService>>();
            _loggerOpen = new Mock<ILogger<OpenRouletteService>>();

            _roulette = new RouletteEntity() { Id = 1 };
            _user = new GamblerEntity("Jose Carlos", 50000) { Id = 1 };
            _bet =  new(100, BetType.Color, RouletteColor.Red, null, _user.Id, _roulette.Id);
        }

        /*
          1. Ruleta no existe
             • Dado una ruletaId inexistente
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Error creating bet: Roulette not found.")
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldReturnRouletteNotFound_WhenRouletteDoesNotExist()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: Roulette not found.");
        }

        /*
          2. Usuario no existe
             • Dado un userId inexistente
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Error creating bet: User not found.")
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldReturnUserNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: User not found.");
        }

        /*
          3. Ruleta cerrada o creada
             • Dado una ruleta que no está abierta
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Roulette is not open for bets.")
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldReturnError_WhenRouletteIsNotOpen()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: Roulette is not open for bets.");
        }

        /*
          4. Usuario sin crédito suficiente
             • Dado un usuario sin crédito suficiente
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Insufficient credits.")
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldReturnError_WhenUserHasInsufficientCredit()
        {
            // Arrange
            var user = new GamblerEntity("Jose Carlos", 99) { Id = 1 };
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            await new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: Not enough credit to perform this operation.");
        }

        /*
          5. Apuesta duplicada en la misma ruleta
             • Dado un usuario que ya tiene una apuesta en la ruleta
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Error creating bet: You already placed a bet on this roulette.")
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldReturnError_WhenUserAlreadyPlacedBetOnSameRoulette()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _betRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<BetEntity, bool>>>())).ReturnsAsync(_bet);

            await new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: You already placed a bet on this roulette.");
        }

        /*
          6. Monto inválido (menor a 1 o mayor a 10.000)
             • Dado un monto de apuesta inválido
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Bet amount must be between 1 and 10,000.")
        */
        [Theory]
        [InlineData(0)]
        [InlineData(10001)]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldThrowInvalidBetAmountException_WhenAmountIsOutOfRange(int amount)
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            await new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(amount, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: Bet amount must be between $ 1,00 and $ 10.000,00.");
        }

        /*
          7. Tipo de apuesta inválido (ni color ni número)
             • Dado un tipo de apuesta inválido
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail("Invalid bet type.")
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldThrowInvalidBetTypeException_WhenBetTypeIsInvalid()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            await new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color + 10, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: Invalid bet type.");
        }

        /*
          8. Apuesta válida por número
             • Dado un tipo de apuesta válido por número
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Success con los datos correctos
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldCreateNumberBetSuccessfully_WhenValidRequest()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            await new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Color, nameof(RouletteColor.Red), 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Bet created successfully.");
        }

        /*
          9. Apuesta válida por número
             • Dado un tipo de apuesta válido por número
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Success con los datos correctos
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldCreateColorBetSuccessfully_WhenValidRequest()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);

            await new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Number, "5", 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Bet created successfully.");
        }

        /*
          9. Excepción inesperada
             • Dado una excepción inesperada al crear la apuesta
             • Cuando se ejecuta el método Execute
             • Entonces se debe retornar CreateBetResponse.Fail con el mensaje de la excepción
        */
        [Fact]
        [Trait("Category", "CreateBet")]
        public async Task Execute_ShouldRollbackTransaction_WhenUnexpectedErrorOccurs()
        {
            // Arrange
            _gamblerRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<GamblerEntity, bool>>>())).ReturnsAsync(_user);
            _rouletteRepository.Setup(r => r.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).ReturnsAsync(_roulette);
            _betRepository.Setup(r => r.AddAsync(It.IsAny<BetEntity>())).Throws(new Exception("Unexpected error."));

            await   new OpenRouletteService(_rouletteRepository.Object, _unitOfWork.Object, _redis.Object, _loggerOpen.Object).ExecuteAsync(1);

            var service = new CreateBetService(_betRepository.Object, _gamblerRepository.Object, _rouletteRepository.Object, _unitOfWork.Object, _loggerBet.Object);
            var request = new CreateBetRequest(100, BetType.Number, "5", 1);

            // Act
            var action = await service.ExecuteAsync(1, request);

            // Assert
            action.Message.Should().Be("Error creating bet: Unexpected error.");
        }
    }
}

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Roulette.Application.Models;
using Roulette.Application.RouletteServices;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;

namespace Roulette.Application.Test
{
    public class CreateRouletteTest
    {
        private readonly Mock<IRouletteRepository> _repository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IRedisCacheService> _redis;

        private readonly Mock<ILogger<CreateRouletteService>> _logger;

        public CreateRouletteTest()
        {
            _repository = new Mock<IRouletteRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _redis = new Mock<IRedisCacheService>();

            _logger = new Mock<ILogger<CreateRouletteService>>();
        }

        /*
         1.	Debe crear la ruleta correctamente
            •	Dado una una petición para crear una ruleta 
            •	Cuando se llama al método Execute del servicio CreateRoulette
            •	Entonces se debe devolver un RouletteResponse con Id no null, Status "Created", CreatedAt fecha actual y Message "Roulette created successfully."
        */
        [Fact]
        [Trait("Category", "CrearRoulette")]
        public async Task Execute_ShouldCreateRouletteSuccessfully()
        {
            // Arrange
            var service = new CreateRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object, _logger.Object);

            // Act
            var response = await service.ExecuteAsync();

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Code.Should().Be(AppCodes.Roulette.ROULETTE_CREATED);
            response.Message.Should().Be("Roulette created successfully.");
        }

        /*
         2.	Debe devolver error si crear ruleta lanza excepción
            •	Dado una una petición para crear una ruleta 
            •	Cuando se llama al método Execute del servicio CreateRoulette y el repositorio lanza una excepción
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, CreatedAt null y Message "Error creating roulette: {mensaje de la excepción}"
        */
        [Fact]
        [Trait("Category", "CrearRoulette")]
        public async Task Execute_WhenAddThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            _repository.Setup(r => r.AddAsync(It.IsAny<RouletteEntity>()))
                    .Throws(new Exception("DB error"));

            var service = new CreateRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object, _logger.Object);

            // Act
            var response = await service.ExecuteAsync();

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
            response.Message.Should().Be("Error creating roulette: DB error");
        }

        /*
         3.	Debe devolver error si Commit lanza excepción
            •	Dado una una petición para crear una ruleta 
            •	Cuando se llama al método Execute del servicio CreateRoulette y el UnitOfWork lanza una excepción al hacer Commit
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, CreatedAt null y Message "Error creating roulette: {mensaje de la excepción}"
        */
        [Fact]
        [Trait("Category", "CrearRoulette")]
        public async Task Execute_WhenCommitThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            _unitOfWork.Setup(u => u.CommitTransactionAsync())
                      .Throws(new Exception("Transaction error"));

            var service = new CreateRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object, _logger.Object);

            // Act
            var response = await service.ExecuteAsync();

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.System.INTERNAL_ERROR);
            response.Message.Should().Be("Error creating roulette: Transaction error");
        }

        /*
         4.	Debe llamar a Add y Commit exactamente una vez cada uno
            •	Dado una una petición para crear una ruleta
            •	Cuando se llama al método Execute del servicio CreateRoulette y funciona correctamente
            •	Entonces se debe llamar al método Add del repositorio exactamente una vez y al método Commit del UnitOfWork exactamente una vez
        */
        [Fact]
        [Trait("Category", "CrearRoulette")]
        public async Task Execute_ShouldCallAddAndCommitOnce()
        {
            // Arrange
            var service = new CreateRouletteService(_repository.Object, _unitOfWork.Object, _redis.Object, _logger.Object);

            // Act
            await service.ExecuteAsync();

            // Assert
            _repository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.RouletteEntity>()), Times.Once);
            _unitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
        }
    }
}

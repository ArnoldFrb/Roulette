using FluentAssertions;
using Moq;
using Roulette.Application.RouletteServices;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Contracts.Repositories.Base;
using Roulette.Domain.Contracts.Services;
using Roulette.Domain.Entities;
using System.Linq.Expressions;

namespace Roulette.Application.Test
{
    public class CloseRouletteTest
    {
        private readonly RouletteEntity _roulette;
        private readonly Mock<IRouletteRepository> _rouletteR;
        private readonly Mock<IUserRepository> _userR;
        private readonly Mock<IBetRepository> _betR;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UserEntity _user;
        private readonly List<BetEntity> _bet;

        public CloseRouletteTest()
        {

            _rouletteR = new Mock<IRouletteRepository>();
            _userR = new Mock<IUserRepository>();
            _betR = new Mock<IBetRepository>();
            _unitOfWork = new Mock<IUnitOfWork>();

            _user = new UserEntity("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };
            _roulette = new RouletteEntity() { Id = 1};

            _bet =
            [
                new(100, BetType.Color, RouletteColor.Red, null, _user, _roulette) { Id = 1 },
            ];
        }

        /*
         1.	Debe devolver error si la ruleta no existe
            •	Dado una una petición para cerrar una ruleta con Id 5 que no existe en la base de datos
            •	Cuando se llama al método Execute del servicio OpenRoulette
            •	Entonces se debe devolver un RouletteResponse con Id null, Status null, Date null y Message "Roulette not found."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public void Execute_ShouldReturnNotFound_WhenRouletteDoesNotExist()
        {
            // Arrange
            _rouletteR.Setup(r => r.Find(It.IsAny<int>())).Returns(_roulette);
            var service = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = service.Execute(5);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.ClosedAt.Should().BeNull();
            response.Message.Should().Be("Roulette not found.");
        }

        /*
         2.	Debe cerrar la ruleta correctamente
            •	Dado una una petición para cerrar una ruleta con Id 1 que existe en la base de datos
            •	Cuando se llama al método Execute del servicio CloseRoulette
            •	Entonces se debe devolver un RouletteResponse con Id no null, Status "Closed", Date fecha actual de apertura y Message "Roulette opened successfully."
        */
        [Fact]
        [Trait("Category", "CloseRoulette")]
        public void Execute_ShouldReturnBets_WhenRouletteHasBets()
        {
            // Arrange
            _rouletteR.Setup(r => r.FindSingleOrDefault(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Returns(_roulette);
            _betR.Setup(r => r.FindBy(It.IsAny<Expression<Func<BetEntity, bool>>>())).Returns(_bet);

            var serviceO = new OpenRouletteService(_rouletteR.Object, _unitOfWork.Object);
            serviceO.Execute(1);

            var serviceC = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = serviceC.Execute(1);

            // Assert
            response.Id.Should().Be(1);
            response.Status.Should().Be("Closed");
            response.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            response.Bets.Should().NotBeNull();
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
        public void Execute_ShouldCloseRoulette_WhenNoBetsExist()
        {
            // Arrange
            _rouletteR.Setup(r => r.FindSingleOrDefault(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Returns(_roulette);
            _betR.Setup(r => r.FindBy(It.IsAny<Expression<Func<BetEntity, bool>>>())).Returns([]);

            var serviceO = new OpenRouletteService(_rouletteR.Object, _unitOfWork.Object);
            serviceO.Execute(1);

            var serviceC = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = serviceC.Execute(1);

            // Assert
            response.Id.Should().Be(1);
            response.Status.Should().Be("Closed");
            response.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            response.Bets.Should().BeEmpty();
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
        public void Execute_WhenFindThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            _rouletteR.Setup(r => r.FindSingleOrDefault(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Throws(new Exception("DB error."));
            var service = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = service.Execute(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.ClosedAt.Should().BeNull();
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
        public void Execute_WhenFindByThrowsException_ShouldReturnErrorResponse()
        {
            // Arrange
            _rouletteR.Setup(r => r.FindSingleOrDefault(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Returns(_roulette);
            _betR.Setup(r => r.FindBy(It.IsAny<Expression<Func<BetEntity, bool>>>())).Throws(new Exception("Search error."));
            var service = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = service.Execute(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.ClosedAt.Should().BeNull();
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
        public void Execute_ShouldReturnError_WhenRouletteIsNotOpen()
        {
            // Arrange
            _rouletteR.Setup(r => r.FindSingleOrDefault(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Returns(_roulette);
            _betR.Setup(r => r.FindBy(It.IsAny<Expression<Func<BetEntity, bool>>>())).Returns(_bet);
            var service = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = service.Execute(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.ClosedAt.Should().BeNull();
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
        public void Execute_ShouldReturnErrorMessage_WhenExceptionOccurs()
        {
            // Arrange
            _rouletteR.Setup(r => r.FindSingleOrDefault(It.IsAny<Expression<Func<RouletteEntity, bool>>>())).Throws(new Exception("Db Error."));
            var service = new CloseRouletteService(_rouletteR.Object, _betR.Object, _userR.Object, _unitOfWork.Object);

            // Act
            var response = service.Execute(1);

            // Assert
            response.Id.Should().BeNull();
            response.Status.Should().BeNull();
            response.ClosedAt.Should().BeNull();
            response.Message.Should().Be("Error closing roulette: Db Error.");
        }
    }
}

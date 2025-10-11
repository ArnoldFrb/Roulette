using FluentAssertions;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Test
{
    public class RouletteTest
    {
        private readonly RouletteEntity _roulette;

        public RouletteTest()
        {
            _roulette = new RouletteEntity() { Id = 1};
        }
        /// <summary>
        ///  OPEN ROULETTE TESTS
        /// </summary>

        /*
         1.	Abrir apuesta desde estado Created
            •	Dado una ruleta con estado Created
            •	Cuando se abre la apuesta
            •	Entonces el estado de la ruleta es Open y la fecha de apertura se actualiza.
        */
        [Fact]
        public void OpenBet_WithCreatedStatus_ShouldSetStatusToOpenAndUpdateOpenedAt()
        {
            // Act
            _roulette.OpenRoulette();

            // Assert
            _roulette.Status.Should().Be(RouletteStatus.Open);
            _roulette.OpenedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        /*
         2.	Abrir apuesta con estado abierto
            •	Dado una ruleta con estado Open
            •	Cuando se abre la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "Expected status Created, but current is Open."
        */
        [Fact]
        public void OpenBet_WithOpenStatus_ShouldThrowException()
        {
            // Arrange
            _roulette.OpenRoulette();

            // Act
            var action = () => _roulette.OpenRoulette();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("Expected status Created, but current is Open.");
        }

        /*
         3.	Abrir apuesta con estado Closed
            •	Dado una ruleta con estado Closed
            •	Cuando se abre la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "The roulette is already closed."
        */
        [Fact]
        public void OpenBet_WithClosedStatus_ShouldThrowException()
        {
            // Arrange
            _roulette.OpenRoulette();
            _roulette.CloseRoulette();

            // Act
            var action = () => _roulette.OpenRoulette();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("The roulette is already closed.");
        }


        /// <summary>
        ///  CLOSE ROULETTE TESTS
        /// </summary>

        /*
         1.	Cerrar apuesta correctamente
            •	Dado una ruleta con estado distinto de Closed
            •	Cuando se cierra la apuesta
            •	Entonces el estado de la ruleta es Closed y la fecha de cierre se actualiza.
        */
        [Fact]
        public void CloseBet_WithOpenStatus_ShouldSetStatusToClosedAndUpdateClosedAt()
        {
            // Arrange
            _roulette.OpenRoulette();

            // Act
            _roulette.CloseRoulette();

            // Assert
            _roulette.Status.Should().Be(RouletteStatus.Closed);
            _roulette.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        /*
         2.	Cerrar apuesta con estado Created
            •	Dado una ruleta con estado Created
            •	Cuando se cierra la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "Expected status Open, but current is Created."
        */
        [Fact]
        public void CloseBet_WithCreatedStatus_ShouldThrowException()
        {
            // Arrange
            var roulette = _roulette;

            // Act
            var action = () => roulette.CloseRoulette();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("Expected status Open, but current is Created.");
        }

        /*
         3.	Cerrar apuesta con estado Closed
            •	Dado una ruleta con estado Closed
            •	Cuando se cierra la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "The roulette is already closed."
        */
        [Fact]
        public void CloseBet_WithClosedStatus_ShouldThrowException()
        {
            // Arrange
            _roulette.OpenRoulette();
            _roulette.CloseRoulette();

            // Act
            var action = () => _roulette.CloseRoulette();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("The roulette is already closed.");
        }


        /// <summary>
        ///  CREATE ROULETTE TESTS
        /// </summary>

        /*
         1.	Crear nueva ruleta
            •	Dado una petición de creación de una ruleta
            •	Cuando se crea una nueva instancia de Roulette
            •	Entonces las propiedades deben inicializarse correctamente
        */
        [Fact]
        public void RouletteConstructor_WithValidAmount_ShouldInitializeProperties()
        {
            // Arrange
            _roulette.OpenRoulette();

            // Act
            _roulette.CloseRoulette();

            // Assert
            _roulette.NumberWinner.Should().BeInRange(0, 36);
            _roulette.ColorWinner.Should().BeOneOf(RouletteColor.Red, RouletteColor.Black);
            _roulette.Status.Should().Be(RouletteStatus.Closed);
            _roulette.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            _roulette.OpenedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            _roulette.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }
    }
}

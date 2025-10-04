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
            // Arrange
            var roulette = _roulette;

            // Act
            roulette.OpenBet();

            // Assert
            roulette.Status.Should().Be(BetStatus.Open);
            roulette.OpenedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        /*
         2.	Abrir apuesta con estado abierto
            •	Dado una ruleta con estado Open
            •	Cuando se abre la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "Bet is not Created."
        */
        [Fact]
        public void OpenBet_WithOpenStatus_ShouldThrowException()
        {
            // Arrange
            var roulette = _roulette;
            roulette.OpenBet();

            // Act
            var action = () => roulette.OpenBet();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("Bet is not Created.");
        }

        /*
         3.	Abrir apuesta con estado Closed
            •	Dado una ruleta con estado Closed
            •	Cuando se abre la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "The bet is Closed."
        */
        [Fact]
        public void OpenBet_WithClosedStatus_ShouldThrowException()
        {
            // Arrange
            var roulette = _roulette;
            roulette.OpenBet();
            roulette.CloseBet();

            // Act
            var action = () => roulette.OpenBet();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("The bet is Closed.");
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
            var roulette = _roulette;
            roulette.OpenBet();

            // Act
            roulette.CloseBet();

            // Assert
            roulette.Status.Should().Be(BetStatus.Closed);
            roulette.ClosedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }

        /*
         2.	Cerrar apuesta con estado Created
            •	Dado una ruleta con estado Created
            •	Cuando se cierra la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "Bet is not Open."
        */
        [Fact]
        public void CloseBet_WithCreatedStatus_ShouldThrowException()
        {
            // Arrange
            var roulette = _roulette;

            // Act
            var action = () => roulette.CloseBet();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("Bet is not Open.");
        }

        /*
         3.	Cerrar apuesta con estado Closed
            •	Dado una ruleta con estado Closed
            •	Cuando se cierra la apuesta
            •	Entonces lanza InvalidRouletteStatusException con mensaje "The bet is Closed."
        */
        [Fact]
        public void CloseBet_WithClosedStatus_ShouldThrowException()
        {
            // Arrange
            var roulette = _roulette;
            roulette.OpenBet();
            roulette.CloseBet();

            // Act
            var action = () => roulette.CloseBet();

            // Assert
            action.Should().Throw<InvalidRouletteStatusException>()
                .WithMessage("The bet is Closed.");
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
            // Act
            var roulette = _roulette;

            // Assert
            roulette.NumberWinner.Should().BeInRange(0, 36);
            roulette.ColorWinner.Should().BeOneOf(BetColor.Red, BetColor.Black);
            roulette.Status.Should().Be(BetStatus.Created);
            roulette.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            roulette.OpenedAt.Should().Be(DateTime.MinValue);
            roulette.ClosedAt.Should().Be(DateTime.MinValue);
        }

        /*
         2.	Color ganador para número dentro de rango
            •	Dado un número ganador mayor que 0 o menor que 36
            •	Cuando se obtiene el color ganador
            •	Entonces debería manejarse el caso (según la lógica de negocio).
        */
        [Fact]
        public void IsValidNumberWinner_WithValidNumber_ShouldReturnNumber()
        {
            // Arrange
            const int numberWinner = 15;

            // Act
            var number = RouletteEntity.IsValidNumberWinner(numberWinner);

            // Assert
            number.Should().Be(numberWinner);
        }

        /*
         3.	Validar número ganador para número fuera de rango
            •	Dado un número ganador menor que 0 o mayor que 36
            •	Cuando se obtiene el color ganador
            •	Entonces debería manejarse el caso (según la lógica de negocio).
        */
        [Theory]
        [InlineData(37)]
        [InlineData(50)]
        [InlineData(100)]
        [InlineData(-10)]
        [InlineData(-100)]
        public void IsValidNumberWinner_WithOutOfRangeValue_ShouldThrowException(int numberWinner)
        {
            // Act
            var action = () => RouletteEntity.IsValidNumberWinner(numberWinner);

            // Assert
            action.Should().Throw<InvalidNumberWinnerException>().WithMessage("Invalid Number. Must be between 0 and 36.");
        }

        /*
         4.	Validar color ganador para número par
            •	Dado un número ganador par
            •	Cuando se genera el color ganador
            •	Entonces el color debe ser Black
        */
        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(10)]
        [InlineData(36)]
        public void GetWinnerColor_WithEvenNumber_ShouldReturnBlack(int numberWinner)
        {
            // Act
            var color = RouletteEntity.GetWinnerColor(numberWinner);

            // Assert
            color.Should().Be(BetColor.Black);
        }

        /*
         5.	Validar color ganador para número impar
            •	Dado un número ganador impar
            •	Cuando se genera el color ganador
            •	Entonces el color debe ser Red
        */
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(15)]
        [InlineData(21)]
        [InlineData(35)]
        public void GetWinnerColor_WithOddNumber_ShouldReturnRed(int numberWinner)
        {
            // Act
            var color = RouletteEntity.GetWinnerColor(numberWinner);

            // Assert
            color.Should().Be(BetColor.Red);
        }
    }
}

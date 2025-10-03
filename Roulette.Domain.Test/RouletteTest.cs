using FluentAssertions;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;
using System;

namespace Roulette.Domain.Test
{
    public class RouletteTest
    {
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
        public void OpenBet()
        {
            // Arrange
            var roulette = new Entities.Roulette(100) { Id = 1 };

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
            •	Entonces lanza GenericException con mensaje "Bet is not Created."
        */
        [Fact]
        public void OpenBetOpened()
        {
            // Arrange
            var roulette = new Entities.Roulette(100) { Id = 1 };
            roulette.OpenBet();

            // Act
            var action = () => roulette.OpenBet();

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Bet is not Created.");
        }

        /*
         2.	Abrir apuesta con estado Closed
            •	Dado una ruleta con estado Closed
            •	Cuando se abre la apuesta
            •	Entonces lanza GenericException con mensaje "The bet is Closed."
        */
        [Fact]
        public void OpenBetClosed()
        {
            // Arrange
            var roulette = new Entities.Roulette(100) { Id = 1 };
            roulette.OpenBet();
            roulette.CloseBet();

            // Act
            var action = () => roulette.OpenBet();

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("The bet is Closed.");
        }


        /// <summary>
        ///  CLOSE ROULETTE TESTS
        /// </summary>

        /*
         3.	Cerrar apuesta correctamente
            •	Dado una ruleta con estado distinto de Closed
            •	Cuando se cierra la apuesta
            •	Entonces el estado de la ruleta es Closed y la fecha de cierre se actualiza.
        */
        [Fact]
        public void CloseBet()
        {
            // Arrange
            var roulette = new Entities.Roulette(100) { Id = 1 };
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
            •	Entonces lanza GenericException con mensaje "Bet is not Open."
        */
        [Fact]
        public void ClosedBetCreated()
        {
            // Arrange
            var roulette = new Entities.Roulette(100) { Id = 1 };

            // Act
            var action = () => roulette.CloseBet();

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Bet is not Open.");
        }

        /*
         3.	Cerrar apuesta con estado Closed
            •	Dado una ruleta con estado Closed
            •	Cuando se cierra la apuesta
            •	Entonces lanza GenericException con mensaje "The bet is Closed."
        */
        [Fact]
        public void CloseBetClosed()
        {
            // Arrange
            var roulette = new Entities.Roulette(100) { Id = 1 };
            roulette.OpenBet();
            roulette.CloseBet();

            // Act
            var action = () => roulette.CloseBet();

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("The bet is Closed.");
        }


        /// <summary>
        ///  CREATE ROULETTE TESTS
        /// </summary>

        /*
         1.	Crear nueva ruleta
            •	Dado un monto válido
            •	Cuando se crea una nueva instancia de Roulette
            •	Entonces las propiedades deben inicializarse correctamente
        */
        [Fact]
        public void CreateRoulette()
        {
            // Arrange
            const int amount = 100;

            // Act
            var roulette = new Entities.Roulette(amount) { Id = 1 };

            // Assert
            roulette.Amount.Should().Be(amount);
            roulette.NumberWinner.Should().BeInRange(0, 36);
            roulette.ColorWinner.Should().BeOneOf(BetColor.Red, BetColor.Black);
            roulette.Status.Should().Be(BetStatus.Created);
            roulette.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            roulette.OpenedAt.Should().Be(DateTime.MinValue);
            roulette.ClosedAt.Should().Be(DateTime.MinValue);
        }

        /*
         2.	Validar color ganador para número par
            •	Dado un número ganador par
            •	Cuando se genera el color ganador
            •	Entonces el color debe ser Black
        */

        [Fact]
        public void EvenNumber()
        {

            // Arrange
            const int numberWinner = 4;

            // Act
            var color = GetWinnerColor(numberWinner);

            // Assert
            color.Should().Be(BetColor.Black);
        }

        /*
         3.	Validar color ganador para número impar
            •	Dado un número ganador impar
            •	Cuando se genera el color ganador
            •	Entonces el color debe ser Red
        */
        [Fact]
        public void OddNumber()
        {
            // Arrange
            const int numberWinner = 15;

            // Act
            var color = GetWinnerColor(numberWinner);

            // Assert
            color.Should().Be(BetColor.Red);
        }

        private static BetColor GetWinnerColor(int numberWinner) => (numberWinner % 2 == 0) ? BetColor.Black : BetColor.Red;
    }
}

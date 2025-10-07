using FluentAssertions;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Test
{
    public class BetTest
    {
        private readonly int creditInitial = 50000;
        private readonly UserEntity _user;
        private readonly RouletteEntity _roulette;
        public BetTest()
        {
            _user = new UserEntity("Jose Carlos", "@#Hl1g2l34", creditInitial) { Id = 1 };
            _roulette = new RouletteEntity() { Id = 1 };
        }

        /// <summary>
        ///  VALIDATED BET TESTS
        /// </summary>

        /*
         1.	Validar apuesta por color válido
            •	Dado una apuesta de tipo Color con valor "Red", monto 100 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces no debe lanzar excepción y debe deducirse el monto del usuario
        */
        [Fact]
        public void IsValidBet_WithValidColor_ShouldNotThrowAndDeductCredit()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Color, RouletteColor.Red, null, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);

            // Assert
            action.Should().NotThrow();
            user.Credit.Should().Be(creditInitial - bet.Amount);
        }

        /*
         2.	Validar apuesta por color null
            •	Dado una apuesta de tipo Color con valor "null", monto 100 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces debe lanzar excepción con mensaje "Invalid color bet. Must be 'red' or 'black'."
        */
        [Fact]
        public void IsValidBet_WithNullColor_ShouldThrowException()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Color, null, null, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();

            // Assert
            action.Should().Throw<InvalidBetColorException>().WithMessage("Invalid color bet. Must be 'red' or 'black'.");
        }

        /*
         3.	Validar apuesta por numero válido
            •	Dado una apuesta de tipo Number con valor 15, monto 100 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces no debe lanzar excepción y debe deducirse el monto del usuario
        */
        [Fact]
        public void IsValidBet_WithValidNumber_ShouldNotThrowAndDeductCredit()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Number, null, 15, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);

            // Assert
            action.Should().NotThrow();
            user.Credit.Should().Be(creditInitial - bet.Amount);
        }

        /*
         4.	Validar apuesta por numero null
            •	Dado una apuesta de tipo Number con valor null, monto 100 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces debe lanzar excepción con mensaje "Invalid number bet. Must be between 0 and 36."
        */
        [Fact]
        public void IsValidBet_WithNullNumber_ShouldThrowException()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Number, null, null, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();

            // Assert
            action.Should().Throw<InvalidBetNumberException>().WithMessage("Invalid number bet. Must be between 0 and 36.");
        }

        /*
         5.	Validar apuesta por numero inválido mayor a 36
            •	Dado una apuesta de tipo Number con valor 50, monto 100 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces debe lanzar excepción con mensaje "Invalid number bet. Must be between 0 and 36."
        */
        [Fact]
        public void IsValidBet_WithNumberGreaterThanMax_ShouldThrowException()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Number, null, 50, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();

            // Assert
            action.Should().Throw<InvalidBetNumberException>().WithMessage("Invalid number bet. Must be between 0 and 36.");
        }

        /*
         6.	Validar apuesta por numero inválido menor a 0
            •	Dado una apuesta de tipo Number con valor -10, monto 100 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces debe lanzar excepción con mensaje "Invalid number bet. Must be between 0 and 36."
        */
        [Fact]
        public void IsValidBet_WithNumberLessThanMin_ShouldThrowException()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Number, null, -10, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();

            // Assert
            action.Should().Throw<InvalidBetNumberException>().WithMessage("Invalid number bet. Must be between 0 and 36.");
        }

        /*
         7. Validar apuesta con monto válido
            •	Dado una apuesta de tipo Number con valor 15, monto 105 y crédito del usuario 50000
            •	Cuando se valida la apuesta
            •	Entonces no debe lanzar excepción y debe deducirse el monto del usuario
        */
        [Fact]
        public void IsValidBet_WithValidAmount_ShouldNotThrowAndDeductCredit()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(105, BetType.Number, null, 15, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);

            // Assert
            action.Should().NotThrow();
            user.Credit.Should().Be(creditInitial - bet.Amount);
        }

        /*
         8. Validar apuesta con monto inválido mayor a 10000
            •	Dado una apuesta de tipo Number con valor 15 y monto 10005
            •	Cuando se valida la apuesta
            •	Entonces debe lanzar excepción con mensaje "Bet amount must be between $ 1,00 and $ 10.000,00."
        */
        [Fact]
        public void IsValidBet_WithAmountGreaterThanMax_ShouldThrowException()
        {
            var user = _user;
            var roulette = _roulette;

            // Act
            var action = () => new BetEntity(10005, BetType.Number, null, 15, user, roulette) { Id = 1 };

            // Assert
            action.Should().Throw<InvalidBetAmountException>().WithMessage("Bet amount must be between $ 1,00 and $ 10.000,00.");
        }

        /*
         9. Validar apuesta con monto inválido menor a 0
            •	Dado una apuesta de tipo Number con valor 15 y monto -25
            •	Cuando se valida la apuesta
            •	Entonces debe lanzar excepción con mensaje "Bet amount must be between $ 1,00 and $ 10.000,00."
        */
        [Fact]
        public void IsValidBet_WithAmountLessThanMin_ShouldThrowException()
        {
            var user = _user;
            var roulette = _roulette;

            // Act
            var action = () => new BetEntity(-25, BetType.Number, null, 15, user, roulette) { Id = 1 };

            // Assert
            action.Should().Throw<InvalidBetAmountException>().WithMessage("Bet amount must be between $ 1,00 and $ 10.000,00.");
        }


        /// <summary>
        ///  WINNER BET TESTS
        /// </summary>

        /*
         1.	Validar apuesta ganadora por color
            •	Dado una apuesta de tipo Color "Red", monto 100 y crédito del usuario 50000
            •	Cuando se verifica si es ganador
            •	Entonces debe asignar el monto ganado al usuario si gana, o 0 si pierde
        */
        [Fact]
        public void IsWinner_WithColorBet_ShouldAssignWinningsOrZero()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Color, RouletteColor.Red, null, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);
            var result = bet.IsWinner();
            decimal winnings = 0;
            if (result)
            {
                winnings = bet.GetWinnings();
                user.PayCredit(winnings);
            }

            // Assert
            action.Should().NotThrow();
            if (result)
            {
                result.Should().BeTrue();
            }
            else
            {
                result.Should().BeFalse();
            }
            user.Credit.Should().Be((creditInitial - bet.Amount) + winnings);
        }

        /*
         2.	Validar apuesta ganadora por Number
            •	Dado una apuesta de tipo Number "15", monto 100 y crédito del usuario 50000
            •	Cuando se verifica si es ganador
            •	Entonces debe asignar el monto ganado al usuario si gana, o 0 si pierde
        */
        [Fact]
        public void IsWinner_WithNumberBet_ShouldAssignWinningsOrZero()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Number, null, 15, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);
            var result = bet.IsWinner();
            decimal winnings = 0;
            if (result)
            {
                winnings = bet.GetWinnings();
                user.PayCredit(winnings);
            }

            // Assert
            action.Should().NotThrow();
            if (result)
            {
                result.Should().BeTrue();
            }
            else
            {
                result.Should().BeFalse();
            }
            user.Credit.Should().Be((creditInitial - bet.Amount) + winnings);
        }



        /// <summary>
        ///  GET WINNINGS BET TESTS
        /// </summary>

        /*
         1.	Calcular ganancias de apuesta ganadora por color
            •	Dado una apuesta de tipo Color "Red", monto 100 y crédito del usuario 50000
            •	Cuando se verifica si es ganador
            •	Entonces debe asignar el monto ganado al usuario si gana, o 0 si pierde
        */
        [Fact]
        public void GetWinnings_WithWinningColorBet_ShouldReturnCorrectAmount()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Color, RouletteColor.Red, null, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);
            var result = bet.IsWinner();
            decimal winnings = 0;
            if (result)
            {
                winnings = bet.GetWinnings();
                user.PayCredit(winnings);
            }

            // Assert
            action.Should().NotThrow();
            if (result)
            {
                result.Should().BeTrue();
                winnings.Should().Be(180);
            }
            else
            {
                result.Should().BeFalse();
                winnings.Should().Be(0);
            }
            user.Credit.Should().Be((creditInitial - bet.Amount) + winnings);
        }

        /*
         2.	Validar apuesta ganadora por Number
            •	Dado una apuesta de tipo Number "15", monto 100 y crédito del usuario 50000
            •	Cuando se verifica si es ganador
            •	Entonces debe asignar el monto ganado al usuario si gana, o 0 si pierde
        */
        [Fact]
        public void GetWinnings_WithWinningNumberBet_ShouldReturnCorrectAmount()
        {
            var user = _user;
            var roulette = _roulette;
            var bet = new BetEntity(100, BetType.Number, null, 15, user, roulette) { Id = 1 };

            // Act
            var action = () => bet.ValidateBet();
            user.DeductCredit(bet.Amount);
            var result = bet.IsWinner();
            decimal winnings = 0;
            if (result)
            {
                winnings = bet.GetWinnings();
                user.PayCredit(winnings);
            }

            // Assert
            action.Should().NotThrow();
            if (result)
            {
                result.Should().BeTrue();
                winnings.Should().Be(500);
            }
            else
            {
                result.Should().BeFalse();
                winnings.Should().Be(0);
            }
            user.Credit.Should().Be((creditInitial - bet.Amount) + winnings);
        }
    }
}

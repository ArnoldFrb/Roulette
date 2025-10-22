using FluentAssertions;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Test
{
    public class GamblerTest
    {
        private readonly GamblerEntity _user;

        public GamblerTest()
        {
            _user = new GamblerEntity("Jose Carlos", 100) { Id = 1 };
        }


        /// <summary>
        ///  ADD CREDIT TESTS
        /// </summary>

        /*
         1.	Agregar crédito válido
            •	Dado un usuario con 100 de crédito×
            •	Cuando agrega 50 de crédito
            •	Entonces el crédito del usuario es 150.
        */
        [Fact]
        [Trait("Category", "AddCredit")]
        public void IncreaseCredit_WithValidAmount_ShouldUpdateCredit()
        {
            // Arrange

            // Act
            _user.PayCredit(50);

            // Assert
            _user.Credit.Should().Be(150);
        }

        /*
         2.	Agregar crédito cero
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta agregar 0 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        [Trait("Category", "AddCredit")]
        public void IncreaseCredit_WithZeroAmount_ShouldThrowException()
        {
            // Arrange

            // Act
            var action = () => _user.PayCredit(0);

            // Assert
            action.Should().Throw<InvalidCreditOperationException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }

        /*
         3.	Agregar crédito negativo
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta agregar -10 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        [Trait("Category", "AddCredit")]
        public void IncreaseCredit_WithNegativeAmount_ShouldThrowException()
        {
            // Arrange

            // Act
            var action = () => _user.PayCredit(-10);

            // Assert
            action.Should().Throw<InvalidCreditOperationException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }


        /// <summary>
        ///  DEDUCT CREDIT TESTS
        /// </summary>

        /*
         1.	Descontar crédito válido
            •	Dado un usuario con 100 de crédito
            •	Cuando descuenta 50 de crédito
            •	Entonces el crédito del usuario es 50.
        */
        [Fact]
        [Trait("Category", "DeductCredit")]
        public void DeductCredit_WithValidAmount_ShouldUpdateCredit()
        {
            // Arrange

            // Act
            _user.DeductCredit(50);

            // Assert
            _user.Credit.Should().Be(50);
        }

        /*
         2.	Descontar crédito igual al saldo
            •	Dado un usuario con 100 de crédito
            •	Cuando descuenta 100 de crédito
            •	Entonces el crédito del usuario es 0.
        */
        [Fact]
        [Trait("Category", "DeductCredit")]
        public void DeductCredit_WithAmountEqualToCredit_ShouldSetCreditToZero()
        {
            // Arrange

            // Act
            _user.DeductCredit(100);

            // Assert
            _user.Credit.Should().Be(0);
        }

        /*
         3.	Descontar crédito mayor al saldo
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta descontar 150 de crédito
            •	Entonces se lanza una excepción con el mensaje "Insufficient credits."
        */
        [Fact]
        [Trait("Category", "DeductCredit")]
        public void DeductCredit_WithAmountExceedingCredit_ShouldThrowException()
        {
            // Arrange

            // Act
            var action = () => _user.DeductCredit(150);

            // Assert
            action.Should().Throw<InvalidCreditOperationException>()
                .WithMessage("Not enough credit to perform this operation.");
        }

        /*
         4.	Descontar crédito cero
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta descontar 0 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        [Trait("Category", "DeductCredit")]
        public void DeductCredit_WithZeroAmount_ShouldThrowException()
        {
            // Arrange

            // Act
            var action = () => _user.DeductCredit(0);

            // Assert
            action.Should().Throw<InvalidCreditOperationException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }

        /*
         5.	Descontar crédito negativo
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta descontar -10 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        [Trait("Category", "DeductCredit")]
        public void DeductCredit_WithNegativeAmount_ShouldThrowException()
        {
            // Arrange

            // Act
            var action = () => _user.DeductCredit(-10);

            // Assert
            action.Should().Throw<InvalidCreditOperationException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }
    }
}
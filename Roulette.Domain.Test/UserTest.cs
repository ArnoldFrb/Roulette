using FluentAssertions;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Test
{
    public class UserTest
    {
        /// <summary>
        ///  PASSWORD TESTS
        /// </summary>

        /*
         1.	Contraseña válida
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando "@#Hl1g2l34"
            •	Entonces la validación es exitosa (no se lanza excepción).
        */
        [Fact]
        public void IsValidPassword()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidPassword("@#Hl1g2l34");

            // Assert
            action.Should().NotThrow();
        }

        /*
         2.	Contraseña inválida
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando "contraseña123"
            •	Entonces se lanza una excepción con el mensaje "Invalid password."
        */
        [Fact]
        public void IsNotValidPassword()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidPassword("contraseña123");

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid password.");
        }

        /*
         3.	Contraseña vacía
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando ""
            •	Entonces se lanza una excepción con el mensaje "Invalid password."
        */
        [Fact]
        public void IsEmptyPassword()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidPassword("");

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid password.");
        }

        /*
         4.	Contraseña nula
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando null
            •	Entonces se lanza una excepción (puede ser por error de referencia o "Invalid password." según implementación).
        */
        [Fact]
        public void IsNullPassword()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidPassword(null!);

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid password.");
        }


        /// <summary>
        ///  USERNAME TESTS
        /// </summary>

        /*
         1.	Nombre de usuario válido
            •	Dado un usuario con nombre "Jose Carlos"
            •	Cuando se valida el nombre ingresando "Jose Carlos"
            •	Entonces la validación es exitosa (no se lanza excepción).
        */
        [Fact]
        public void IsValidUsername()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidUsername("Jose Carlos");

            // Assert
            action.Should().NotThrow();
        }

        /*
         2.	Nombre de usuario inválido
            •	Dado un usuario con nombre "Jose Carlos"
            •	Cuando se valida el nombre ingresando "Carlos Jose"
            •	Entonces se lanza una excepción con el mensaje "Invalid username."
        */
        [Fact]
        public void IsNotValidUsername()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidUsername("Carlos Jose");

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid username.");
        }

        /*
         3.	Nombre de usuario vacío
            •	Dado un usuario con nombre "Jose Carlos"
            •	Cuando se valida el nombre ingresando ""
            •	Entonces se lanza una excepción con el mensaje "Invalid username."
        */
        [Fact]
        public void IsEmptyUsername()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidUsername("");

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid username.");
        }

        /*
         4.	Nombre de usuario nulo
            •	Dado un usuario con nombre "Jose Carlos"
            •	Cuando se valida el nombre ingresando null
            •	Entonces se lanza una excepción (puede ser por error de referencia o "Invalid username." según implementación).
        */
        [Fact]
        public void IsNullUsername()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };

            // Act
            var action = () => user.IsValidUsername(null!);

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid username.");
        }


        /// <summary>
        ///  ADD CREDIT TESTS
        /// </summary>

        /*
         1.	Agregar crédito válido
            •	Dado un usuario con 100 de crédito
            •	Cuando agrega 50 de crédito
            •	Entonces el crédito del usuario es 150.
        */
        [Fact]
        public void AddValidCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            user.AddCredit(50);

            // Assert
            user.Credit.Should().Be(150);
        }

        /*
         2.	Agregar crédito cero
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta agregar 0 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        public void AddZeroCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            var action = () => user.AddCredit(0);

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }

        /*
         3.	Agregar crédito negativo
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta agregar -10 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        public void AddNegativeCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            var action = () => user.AddCredit(-10);

            // Assert
            action.Should().Throw<GenericException>()
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
        public void DeductValidCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            user.DeductCredit(50);

            // Assert
            user.Credit.Should().Be(50);
        }

        /*
         2.	Descontar crédito igual al saldo
            •	Dado un usuario con 100 de crédito
            •	Cuando descuenta 100 de crédito
            •	Entonces el crédito del usuario es 0.
        */
        [Fact]
        public void DeductAmountEqualToCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            user.DeductCredit(100);

            // Assert
            user.Credit.Should().Be(0);
        }

        /*
         3.	Descontar crédito mayor al saldo
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta descontar 150 de crédito
            •	Entonces se lanza una excepción con el mensaje "Insufficient credits."
        */
        [Fact]
        public void DeductAmountExceedingToCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            var action = () => user.DeductCredit(150);

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Insufficient credits.");
        }

        /*
         4.	Descontar crédito cero
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta descontar 0 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        public void DeductZeroCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            var action = () => user.DeductCredit(0);

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }

        /*
         5.	Descontar crédito negativo
            •	Dado un usuario con 100 de crédito
            •	Cuando intenta descontar -10 de crédito
            •	Entonces se lanza una excepción con el mensaje "Invalid credits amount. Must be greater than 0."
        */
        [Fact]
        public void DeductNegativeCredit()
        {
            // Arrange
            var user = new User("Jose Carlos", "@#Hl1g2l34", 100) { Id = 1 };

            // Act
            var action = () => user.DeductCredit(-10);

            // Assert
            action.Should().Throw<GenericException>()
                .WithMessage("Invalid credits amount. Must be greater than 0.");
        }
    }
}
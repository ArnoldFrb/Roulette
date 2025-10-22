using FluentAssertions;
using Roulette.Domain.Entities;
using Roulette.Domain.Entities.Exceptions;

namespace Roulette.Domain.Test
{
    public class CrupierTest
    {
        private readonly CrupierEntity _user;

        public CrupierTest()
        {
            _user = new CrupierEntity("Jose Carlos", "@#Hl1g2l34", true) { Id = 1 };
        }

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
        [Trait("Category", "Password")]
        public void IsValidPassword_WithCorrectPassword_ShouldNotThrow()
        {
            // Arrange
            const string password = "@#Hl1g2l34";

            // Act
            var action = _user.ValidatePassword(password);

            // Assert
            action.Should().BeTrue();
        }

        /*
         2.	Contraseña inválida
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando "contraseña123"
            •	Entonces se debe retornar false.
        */
        [Fact]
        [Trait("Category", "Password")]
        public void IsValidPassword_WithIncorrectPassword_ShouldThrowException()
        {
            // Arrange
            const string password = "contraseña123";

            // Act
            var action = _user.ValidatePassword(password);

            // Assert
            action.Should().BeFalse();
        }

        /*
         3.	Contraseña vacía
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando ""
            •	Entonces se debe retornar false.
        */
        [Fact]
        [Trait("Category", "Password")]
        public void IsValidPassword_WithEmptyPassword_ShouldThrowException()
        {
            // Arrange
            const string password = "";

            // Act
            var action = _user.ValidatePassword(password);

            // Assert
            action.Should().BeFalse();
        }
    }
}
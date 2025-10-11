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
            _user = new CrupierEntity("Jose Carlos", "@#Hl1g2l34") { Id = 1 };
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

            // Act
            var action = _user.ValidatePassword(_user.Password);

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

            // Act
            var action = _user.ValidatePassword("contraseña123");

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

            // Act
            var action = _user.ValidatePassword("");

            // Assert
            action.Should().BeFalse();
        }

        /*
         4.	Contraseña nula
            •	Dado un usuario con contraseña "@#Hl1g2l34"
            •	Cuando se valida la contraseña ingresando null
            •	Entonces se debe retornar false.
        */
        [Fact]
        [Trait("Category", "Password")]
        public void IsValidPassword_WithNullPassword_ShouldThrowException()
        {
            // Arrange

            // Act
            var action = _user.ValidatePassword(null!);

            // Assert
            action.Should().BeFalse();
        }


        /// <summary>
        ///  USERNAME TESTS
        /// </summary>

        /*
         1.	Nombre de usuario vacío
            •	Dado un usuario con nombre "Jose Carlos"
            •	Cuando ingresa un nombre vacío ""
            •	Entonces se lanza una excepción con el mensaje "The credentials are incorrect."
        */
        [Fact]
        [Trait("Category", "Usernames")]
        public void IsValidUsername_WithEmptyUsername_ShouldThrowException()
        {

            // Act
            var action = () => CrupierEntity.IsValidUsername("");

            // Assert
            action.Should().Throw<InvalidUsernameOrPasswordException>()
                .WithMessage("The credentials are incorrect.");
        }

        /*
         2.	Nombre de usuario nulo
            •	Dado un usuario con nombre "Jose Carlos"
            •	Cuando se valida el nombre ingresando null
            •	Entonces se lanza una excepción (puede ser por error de referencia o "The credentials are incorrect." según implementación).
        */
        [Fact]
        [Trait("Category", "Usernames")]
        public void IsValidUsername_WithNullUsername_ShouldThrowException()
        {

            // Act
            var action = () => CrupierEntity.IsValidUsername(null!);

            // Assert
            action.Should().Throw<InvalidUsernameOrPasswordException>()
                .WithMessage("The credentials are incorrect.");
        }
    }
}
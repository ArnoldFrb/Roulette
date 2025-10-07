using Moq;
using FluentAssertions;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;
using Roulette.Application.UserServices;
using Roulette.Application.Models.Requests;
using System.Linq.Expressions;

namespace Roulette.Application.Test
{
    public class AuthenticationTest
    {
        private readonly UserEntity _user;
        public AuthenticationTest()
        {
            _user = new UserEntity("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };
        }

        /*
         1.	Usuario no existe
            •	Dado un usuario con Username "pepe" y Password "password123" que no existe en la base de datos
            •	Cuando se llama al método de autentificación con Username "pepe" y Password "password123"
            •	Entonces se debe devolver un UserResponse con Id null, UserName null y Message "An error occurred during authentication.\nException: User not found"
        */
        [Fact]
        [Trait("Category", "Auth")]
        public void Authenticate_WithNonExistentUser_ShouldReturnUserNotFound()
        {

            // Arrange
            var repository = new Mock<IUserRepository>();

            var service = new AuthenticationService(repository.Object);
            var request = new AuthenticationRequest("pepe", "password123");

            // Act
            var response = service.Authenticate(request);

            // Assert
            response.Id.Should().BeNull();
            response.UserName.Should().BeNull();
            response.Message.Should().Be("An error occurred during authentication.\nException: User not found.");
        }

        /*
         2.	Usuario existe y contraseña válida
            •	Dado un usuario con Username "Jose Carlos" y Password "@#Hl1g2l34" que existe en la base de datos
            •	Cuando se llama al método de autentificación con Username "Jose Carlos" y Password "@#Hl1g2l34"
            •	Entonces se debe devolver un UserResponse con Id 1, UserName Jose Carlos y Message "Authentication successful"
        */
        [Fact]
        [Trait("Category", "Auth")]
        public void Authenticate_WithValidCredentials_ShouldReturnSuccess()
        {

            // Arrange
            var repository = new Mock<IUserRepository>();

            repository.Setup(repo => repo.FindSingleOrDefault(It.IsAny<Expression<Func<UserEntity, bool>>>()))
            .Returns(_user);

            var service = new AuthenticationService(repository.Object);
            var request = new AuthenticationRequest("Jose Carlos", "@#Hl1g2l34");

            // Act
            var response = service.Authenticate(request);

            // Assert
            response.Id.Should().Be(1);
            response.UserName.Should().Be("Jose Carlos");
            response.Message.Should().Be("Authentication successful.");
        }

        /*
         3.	Usuario existe pero contraseña inválida
            •	Dado un usuario con Username "Jose Carlos" y Password "password123" que existe en la base de datos
            •	Cuando se llama al método de autentificación con Username "Jose Carlos" y Password "password123"
            •	Entonces se debe devolver un UserResponse con Id null, UserName Jose Carlos y Message "An error occurred during authentication.\nException: Invalid password"
        */
        [Fact]
        [Trait("Category", "Auth")]
        public void Authenticate_WithInvalidPassword_ShouldReturnAuthenticationError()
        {

            // Arrange
            var repository = new Mock<IUserRepository>();

            repository.Setup(repo => repo.FindSingleOrDefault(It.IsAny<Expression<Func<UserEntity, bool>>>()))
            .Returns(_user);

            var service = new AuthenticationService(repository.Object);
            var request = new AuthenticationRequest("Jose Carlos", "password123");

            // Act
            var response = service.Authenticate(request);

            // Assert
            response.Id.Should().BeNull();
            response.UserName.Should().BeNull();
            response.Message.Should().Be("An error occurred during authentication.\nException: Invalid password.");
        }
    }
}
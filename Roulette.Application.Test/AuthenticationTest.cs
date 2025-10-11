using Moq;
using FluentAssertions;
using Roulette.Domain.Contracts.Repositories;
using Roulette.Domain.Entities;
using Roulette.Application.UserServices;
using Roulette.Application.Models.Requests;
using System.Linq.Expressions;
using Roulette.Application.Models;

namespace Roulette.Application.Test
{
    public class AuthenticationTest
    {
        private readonly Mock<IUserRepository> _repository;
        private readonly UserEntity _user;
        public AuthenticationTest()
        {
            _repository = new Mock<IUserRepository>();
            _user = new UserEntity("Jose Carlos", "@#Hl1g2l34", 50000) { Id = 1 };
        }

        /*
         1.	Usuario no existe
            •	Dado un usuario con Username "pepe" y Password "password123" que no existe en la base de datos
            •	Cuando se llama al método de autentificación con Username "pepe" y Password "password123"
            •	Entonces se debe devolver un UserResponse con Id null, UserName null y Message "Authentication failed: User not found"
        */
        [Fact]
        [Trait("Category", "Auth")]
        public async Task Authenticate_WithNonExistentUser_ShouldReturnUserNotFound()
        {

            // Arrange
            var service = new AuthenticationService(_repository.Object);
            var request = new AuthenticationRequest("pepe", "password123");

            // Act
            var response = await service.ExecuteAsync(request);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.User.USER_NOT_FOUND);
            response.Message.Should().Be("Authentication failed: User not found.");
        }

        /*
         2.	Usuario existe y contraseña válida
            •	Dado un usuario con Username "Jose Carlos" y Password "@#Hl1g2l34" que existe en la base de datos
            •	Cuando se llama al método de autentificación con Username "Jose Carlos" y Password "@#Hl1g2l34"
            •	Entonces se debe devolver un UserResponse con Id 1, UserName Jose Carlos y Message "Authentication successful"
        */
        [Fact]
        [Trait("Category", "Auth")]
        public async Task Authenticate_WithValidCredentials_ShouldReturnsSuccess()
        {

            // Arrange
            _repository.Setup(repo => repo.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
            .ReturnsAsync(_user);

            var service = new AuthenticationService(_repository.Object);
            var request = new AuthenticationRequest("Jose Carlos", "@#Hl1g2l34");

            // Act
            var response = await service.ExecuteAsync(request);

            // Assert
            response.IsSuccess.Should().BeTrue();
            response.Code.Should().Be(AppCodes.Auth.AUTH_SUCCESS);
            response.Message.Should().Be("Authentication successful.");
        }

        /*
         3.	Usuario existe pero contraseña inválida
            •	Dado un usuario con Username "Jose Carlos" y Password "password123" que existe en la base de datos
            •	Cuando se llama al método de autentificación con Username "Jose Carlos" y Password "password123"
            •	Entonces se debe devolver un UserResponse con Id null, UserName Jose Carlos y Message "Authentication failed: Invalid password"
        */
        [Fact]
        [Trait("Category", "Auth")]
        public async Task Authenticate_WithInvalidPassword_ShouldReturnAuthenticationError()
        {

            // Arrange
            _repository.Setup(repo => repo.FindSingleOrDefaultAsync(It.IsAny<Expression<Func<UserEntity, bool>>>()))
            .ReturnsAsync(_user);

            var service = new AuthenticationService(_repository.Object);
            var request = new AuthenticationRequest("Jose Carlos", "password123");

            // Act
            var response = await service.ExecuteAsync(request);

            // Assert
            response.IsSuccess.Should().BeFalse();
            response.Code.Should().Be(AppCodes.Auth.AUTH_FAILED);
            response.Message.Should().Be("Authentication failed: Invalid password.");
        }
    }
}
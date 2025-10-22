using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Roulette.Application.Models.Requests;
using Roulette.Application.Models.Responses.User;
using Roulette.Domain.Contracts.Redis;
using Roulette.Domain.Contracts.Security;
using Roulette.Domain.Contracts.Services.User;
using Roulette.Domain.Entities;
using Roulette.Infrastructure.Data;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Roulette.API.Test.Utils
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile("appsettings.Testing.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();

            });

            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<RouletteDbContext>));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbConnection));

                if (dbConnectionDescriptor != null)
                    services.Remove(dbConnectionDescriptor);

                services.AddSingleton<DbConnection>(_ =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<RouletteDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                });
            });

            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IJwtTokenService>();
                services.RemoveAll<IRedisCacheService>();

                var token = new Mock<IJwtTokenService>();
                token.Setup(x => x.GetJwtToken(It.IsAny<string>(), It.IsAny<int>())).Returns(GenerateValidToken());

                services.AddSingleton(token.Object);

                var redis = new Mock<IRedisCacheService>();
                redis.Setup(x => x.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
                redis.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<List<RouletteEntity>>(), It.IsAny<int?>(), It.IsAny<bool>())).Returns(Task.CompletedTask);
                services.AddSingleton(redis.Object);
            });
        }

        public static string GenerateValidToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-key-123456789012345678901234567890"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "1"),
                new Claim(JwtRegisteredClaimNames.UniqueName, "crupier1"),
                new Claim(ClaimTypes.Role, "Crupier"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "Roulette.API.Test",
                audience: "Roulette.Client.Test",
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixCrupierSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Crupier",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "password1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Crupier",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$Y/GfaK4/pbWdyFLu/J9p/.c7YtDutUUsVQm1O35xtXIKc.UL5IaQO");
        }
    }
}

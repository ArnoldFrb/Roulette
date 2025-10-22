using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Roulette.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Crupier",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$2a$11$erlnEHHme/pAaVi9IR9QMedGTFrV/9ajOsiycj2ejpRIRd04U8A0q");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Crupier",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "password1");
        }
    }
}

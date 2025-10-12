using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Roulette.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roulette",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumberWinner = table.Column<int>(type: "INTEGER", nullable: false),
                    ColorWinner = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roulette", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Crupier",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Crupier", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Crupier_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Gambler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Credit = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gambler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gambler_User_Id",
                        column: x => x.Id,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    BetType = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    Color = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    Number = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Winnings = table.Column<decimal>(type: "TEXT", precision: 10, scale: 2, nullable: false),
                    Result = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    GamblerId = table.Column<int>(type: "INTEGER", nullable: false),
                    RouletteId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bet_Gambler_GamblerId",
                        column: x => x.GamblerId,
                        principalTable: "Gambler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bet_Roulette_RouletteId",
                        column: x => x.RouletteId,
                        principalTable: "Roulette",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Username" },
                values: new object[,]
                {
                    { 1, "crupier1" },
                    { 2, "user0" },
                    { 3, "user1" },
                    { 4, "user2" },
                    { 5, "user3" }
                });

            migrationBuilder.InsertData(
                table: "Crupier",
                columns: new[] { "Id", "Password" },
                values: new object[] { 1, "password1" });

            migrationBuilder.InsertData(
                table: "Gambler",
                columns: new[] { "Id", "Credit" },
                values: new object[,]
                {
                    { 2, 1000m },
                    { 3, 500m },
                    { 4, 300m },
                    { 5, 200m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bet_GamblerId",
                table: "Bet",
                column: "GamblerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bet_RouletteId",
                table: "Bet",
                column: "RouletteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bet");

            migrationBuilder.DropTable(
                name: "Crupier");

            migrationBuilder.DropTable(
                name: "Gambler");

            migrationBuilder.DropTable(
                name: "Roulette");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}

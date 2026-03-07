using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotlights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpotlightRoundEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotlightRoundEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PairingEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RoundId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LeaderDancerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    FollowerDancerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PairingEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PairingEntity_SpotlightRoundEntity_RoundId",
                        column: x => x.RoundId,
                        principalTable: "SpotlightRoundEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PairingEntity_RoundId",
                table: "PairingEntity",
                column: "RoundId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity",
                column: "SessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PairingEntity");

            migrationBuilder.DropTable(
                name: "SpotlightRoundEntity");
        }
    }
}

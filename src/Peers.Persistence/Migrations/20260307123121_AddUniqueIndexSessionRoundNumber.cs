using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexSessionRoundNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity");

            migrationBuilder.CreateIndex(
                name: "IX_SpotlightRoundEntity_SessionId_RoundNumber",
                table: "SpotlightRoundEntity",
                columns: new[] { "SessionId", "RoundNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpotlightRoundEntity_SessionId_RoundNumber",
                table: "SpotlightRoundEntity");

            migrationBuilder.CreateIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity",
                column: "SessionId");
        }
    }
}

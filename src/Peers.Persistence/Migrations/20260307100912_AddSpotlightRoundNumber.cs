using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotlightRoundNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity");

            migrationBuilder.AddColumn<int>(
                name: "RoundNumber",
                table: "SpotlightRoundEntity",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity");

            migrationBuilder.DropColumn(
                name: "RoundNumber",
                table: "SpotlightRoundEntity");

            migrationBuilder.CreateIndex(
                name: "IX_SpotlightRoundEntity_SessionId",
                table: "SpotlightRoundEntity",
                column: "SessionId",
                unique: true);
        }
    }
}

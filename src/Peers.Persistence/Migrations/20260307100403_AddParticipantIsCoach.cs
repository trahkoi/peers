using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddParticipantIsCoach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCoach",
                table: "ParticipantEntity",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCoach",
                table: "ParticipantEntity");
        }
    }
}

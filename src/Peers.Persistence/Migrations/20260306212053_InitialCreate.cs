using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peers.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DancerEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DancerEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsEnded = table.Column<bool>(type: "INTEGER", nullable: false),
                    InviteCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ParticipantEntity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DancerId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DancerName = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    Token = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParticipantEntity_DancerEntity_DancerId",
                        column: x => x.DancerId,
                        principalTable: "DancerEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParticipantEntity_SessionEntity_SessionId",
                        column: x => x.SessionId,
                        principalTable: "SessionEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEntity_DancerId",
                table: "ParticipantEntity",
                column: "DancerId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEntity_SessionId_DancerName",
                table: "ParticipantEntity",
                columns: new[] { "SessionId", "DancerName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantEntity_Token",
                table: "ParticipantEntity",
                column: "Token",
                unique: true,
                filter: "Token IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SessionEntity_InviteCode",
                table: "SessionEntity",
                column: "InviteCode",
                unique: true,
                filter: "InviteCode IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ParticipantEntity");

            migrationBuilder.DropTable(
                name: "DancerEntity");

            migrationBuilder.DropTable(
                name: "SessionEntity");
        }
    }
}

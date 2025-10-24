using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Implementation.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PicturePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayedCount = table.Column<int>(type: "int", nullable: false),
                    WinCount = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    CreatedRoomId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoinedRoomId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerEntityId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Players_PlayerEntityId",
                        column: x => x.PlayerEntityId,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    AsString = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.AsString);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WordLength = table.Column<int>(type: "int", nullable: false),
                    MaxAttempts = table.Column<int>(type: "int", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    JoinerWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Key);
                    table.ForeignKey(
                        name: "FK_Rooms_Players_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Players",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rooms_Players_JoinerId",
                        column: x => x.JoinerId,
                        principalTable: "Players",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_PlayerEntityId",
                table: "Players",
                column: "PlayerEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CreatorId",
                table: "Rooms",
                column: "CreatorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_JoinerId",
                table: "Rooms",
                column: "JoinerId",
                unique: true,
                filter: "[JoinerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Key",
                table: "Rooms",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Words_AsString",
                table: "Words",
                column: "AsString",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}

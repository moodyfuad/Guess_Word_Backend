using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Implementation.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_CreatorId",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedRoomId",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CreatorId",
                table: "Rooms",
                column: "CreatorId",
                unique: true,
                filter: "[CreatorId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_CreatorId",
                table: "Rooms");

            migrationBuilder.AlterColumn<string>(
                name: "CreatorId",
                table: "Rooms",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedRoomId",
                table: "Players",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_CreatorId",
                table: "Rooms",
                column: "CreatorId",
                unique: true);
        }
    }
}

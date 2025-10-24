using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Implementation.Migrations
{
    /// <inheritdoc />
    public partial class AddRoomStateField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "State",
                table: "Rooms");
        }
    }
}

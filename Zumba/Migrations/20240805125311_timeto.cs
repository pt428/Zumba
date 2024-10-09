using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zumba.Migrations
{
    /// <inheritdoc />
    public partial class timeto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeFrom",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TimeTo",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeFrom",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "TimeTo",
                table: "Reservations");
        }
    }
}

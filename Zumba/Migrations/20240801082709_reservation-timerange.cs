using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zumba.Migrations
{
    /// <inheritdoc />
    public partial class reservationtimerange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeRange",
                table: "Reservations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeRange",
                table: "Reservations");
        }
    }
}

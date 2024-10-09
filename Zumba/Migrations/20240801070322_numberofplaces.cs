using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zumba.Migrations
{
    /// <inheritdoc />
    public partial class numberofplaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "NumberOfPlaces",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfPlaces",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

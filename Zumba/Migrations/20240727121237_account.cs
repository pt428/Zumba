using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zumba.Migrations
{
    /// <inheritdoc />
    public partial class account : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BankAccount",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankAccount",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Settings");
        }
    }
}

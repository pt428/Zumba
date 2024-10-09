using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zumba.Migrations
{
    /// <inheritdoc />
    public partial class bnkowner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerOfBankAccount",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerOfBankAccount",
                table: "Settings");
        }
    }
}

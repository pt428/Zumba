using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zumba.Migrations
{
    /// <inheritdoc />
    public partial class changecredit2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Credit",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "AspNetUsers");
        }
    }
}

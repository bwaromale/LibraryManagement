using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Migrations
{
    public partial class AddAddresstoPublisher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Publishers");
        }
    }
}

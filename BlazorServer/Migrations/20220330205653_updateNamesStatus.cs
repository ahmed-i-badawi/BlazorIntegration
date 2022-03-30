using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    public partial class updateNamesStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Machines",
                newName: "CurrentStatus");

            migrationBuilder.RenameColumn(
                name: "ConnectionStatus",
                table: "MachineLogs",
                newName: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentStatus",
                table: "Machines",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "MachineLogs",
                newName: "ConnectionStatus");
        }
    }
}

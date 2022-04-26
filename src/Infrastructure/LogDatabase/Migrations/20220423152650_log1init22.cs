using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogDatabase.Migrations
{
    public partial class log1init22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SiteUserId",
                table: "MachineStatusLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteUserId",
                table: "MachineMessageLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SiteUserId",
                table: "MachineStatusLogs");

            migrationBuilder.DropColumn(
                name: "SiteUserId",
                table: "MachineMessageLogs");
        }
    }
}

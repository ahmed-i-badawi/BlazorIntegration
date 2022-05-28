using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDatabase.Migrations
{
    public partial class siteM2Mmachines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines");

            migrationBuilder.DropIndex(
                name: "IX_Machines_SiteId",
                table: "Machines");

            migrationBuilder.AddColumn<int>(
                name: "ActualNumberOfMachines",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfMachines",
                table: "Sites",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "SiteId",
                table: "Machines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Machines_SiteId",
                table: "Machines",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines");

            migrationBuilder.DropIndex(
                name: "IX_Machines_SiteId",
                table: "Machines");

            migrationBuilder.DropColumn(
                name: "ActualNumberOfMachines",
                table: "Sites");

            migrationBuilder.DropColumn(
                name: "MaxNumberOfMachines",
                table: "Sites");

            migrationBuilder.AlterColumn<int>(
                name: "SiteId",
                table: "Machines",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Machines_SiteId",
                table: "Machines",
                column: "SiteId",
                unique: true,
                filter: "[SiteId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id");
        }
    }
}

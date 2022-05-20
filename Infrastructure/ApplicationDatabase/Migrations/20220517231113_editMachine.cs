using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDatabase.Migrations
{
    public partial class editMachine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines");

            migrationBuilder.DropIndex(
                name: "IX_Machines_SiteId",
                table: "Machines");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines");

            migrationBuilder.DropIndex(
                name: "IX_Machines_SiteId",
                table: "Machines");

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
                column: "SiteId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Machines_Sites_SiteId",
                table: "Machines",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

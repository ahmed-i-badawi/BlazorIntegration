using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDatabase.Migrations
{
    public partial class edituser2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Integrators_IntegratorId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IntegratorId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Integrators",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IntegratorId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Integrators_ApplicationUserId",
                table: "Integrators",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Integrators_AspNetUsers_ApplicationUserId",
                table: "Integrators",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Integrators_AspNetUsers_ApplicationUserId",
                table: "Integrators");

            migrationBuilder.DropIndex(
                name: "IX_Integrators_ApplicationUserId",
                table: "Integrators");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Integrators");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "IntegratorId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IntegratorId",
                table: "AspNetUsers",
                column: "IntegratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Integrators_IntegratorId",
                table: "AspNetUsers",
                column: "IntegratorId",
                principalTable: "Integrators",
                principalColumn: "Id");
        }
    }
}

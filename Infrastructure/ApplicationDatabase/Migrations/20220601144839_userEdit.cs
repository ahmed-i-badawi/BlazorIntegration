using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDatabase.Migrations
{
    public partial class userEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IntegratorId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserType",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Integrators_IntegratorId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IntegratorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IntegratorId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserType",
                table: "AspNetUsers");
        }
    }
}

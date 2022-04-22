using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationDatabase.Migrations
{
    public partial class app2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteZone_Sites_SiteId",
                table: "SiteZone");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteZone_Zones_ZoneId",
                table: "SiteZone");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SiteZone",
                table: "SiteZone");

            migrationBuilder.RenameTable(
                name: "SiteZone",
                newName: "SiteZones");

            migrationBuilder.RenameIndex(
                name: "IX_SiteZone_ZoneId",
                table: "SiteZones",
                newName: "IX_SiteZones_ZoneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SiteZones",
                table: "SiteZones",
                columns: new[] { "SiteId", "ZoneId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SiteZones_Sites_SiteId",
                table: "SiteZones",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteZones_Zones_ZoneId",
                table: "SiteZones",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SiteZones_Sites_SiteId",
                table: "SiteZones");

            migrationBuilder.DropForeignKey(
                name: "FK_SiteZones_Zones_ZoneId",
                table: "SiteZones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SiteZones",
                table: "SiteZones");

            migrationBuilder.RenameTable(
                name: "SiteZones",
                newName: "SiteZone");

            migrationBuilder.RenameIndex(
                name: "IX_SiteZones_ZoneId",
                table: "SiteZone",
                newName: "IX_SiteZone_ZoneId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SiteZone",
                table: "SiteZone",
                columns: new[] { "SiteId", "ZoneId" });

            migrationBuilder.AddForeignKey(
                name: "FK_SiteZone_Sites_SiteId",
                table: "SiteZone",
                column: "SiteId",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SiteZone_Zones_ZoneId",
                table: "SiteZone",
                column: "ZoneId",
                principalTable: "Zones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousePlantMeasurementsApi.Migrations
{
    public partial class AddedDeviceReferenceToMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "Measurements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Measurements_DeviceId",
                table: "Measurements",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Measurements_Devices_DeviceId",
                table: "Measurements",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Measurements_Devices_DeviceId",
                table: "Measurements");

            migrationBuilder.DropIndex(
                name: "IX_Measurements_DeviceId",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "Measurements");
        }
    }
}

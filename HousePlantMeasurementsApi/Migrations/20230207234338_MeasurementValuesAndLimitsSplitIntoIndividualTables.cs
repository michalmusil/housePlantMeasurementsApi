using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousePlantMeasurementsApi.Migrations
{
    public partial class MeasurementValuesAndLimitsSplitIntoIndividualTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LightIntensityHighLimit",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "LightIntensityLowLimit",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "MoistureHighLimit",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "MoistureLowLimit",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "TemperatureHighLimit",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "TemperatureLowLimit",
                table: "Plants");

            migrationBuilder.DropColumn(
                name: "LightIntensity",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Moisture",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "Measurements");

            migrationBuilder.CreateTable(
                name: "MeasurementValueLimits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    LowerLimit = table.Column<double>(type: "float", nullable: false),
                    UpperLimit = table.Column<double>(type: "float", nullable: false),
                    PlantId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementValueLimits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementValueLimits_Plants_PlantId",
                        column: x => x.PlantId,
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasurementValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false),
                    MeasurementId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasurementValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasurementValues_Measurements_MeasurementId",
                        column: x => x.MeasurementId,
                        principalTable: "Measurements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementValueLimits_PlantId",
                table: "MeasurementValueLimits",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasurementValues_MeasurementId",
                table: "MeasurementValues",
                column: "MeasurementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasurementValueLimits");

            migrationBuilder.DropTable(
                name: "MeasurementValues");

            migrationBuilder.AddColumn<double>(
                name: "LightIntensityHighLimit",
                table: "Plants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LightIntensityLowLimit",
                table: "Plants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MoistureHighLimit",
                table: "Plants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MoistureLowLimit",
                table: "Plants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TemperatureHighLimit",
                table: "Plants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TemperatureLowLimit",
                table: "Plants",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LightIntensity",
                table: "Measurements",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Moisture",
                table: "Measurements",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "Measurements",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousePlantMeasurementsApi.Migrations
{
    public partial class RemovedCommunicationIdentifierLengthConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MacHash",
                table: "Devices",
                newName: "MacAddress");

            migrationBuilder.AlterColumn<string>(
                name: "CommunicationIdentifier",
                table: "Devices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MacAddress",
                table: "Devices",
                newName: "MacHash");

            migrationBuilder.AlterColumn<string>(
                name: "CommunicationIdentifier",
                table: "Devices",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}

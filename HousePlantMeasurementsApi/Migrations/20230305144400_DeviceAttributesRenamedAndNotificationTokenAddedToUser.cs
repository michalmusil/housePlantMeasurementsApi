using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HousePlantMeasurementsApi.Migrations
{
    public partial class DeviceAttributesRenamedAndNotificationTokenAddedToUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UUID",
                table: "Devices",
                newName: "CommunicationIdentifier");

            migrationBuilder.RenameColumn(
                name: "AuthHash",
                table: "Devices",
                newName: "MacHash");

            migrationBuilder.AddColumn<string>(
                name: "NotificationToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationToken",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "MacHash",
                table: "Devices",
                newName: "AuthHash");

            migrationBuilder.RenameColumn(
                name: "CommunicationIdentifier",
                table: "Devices",
                newName: "UUID");
        }
    }
}

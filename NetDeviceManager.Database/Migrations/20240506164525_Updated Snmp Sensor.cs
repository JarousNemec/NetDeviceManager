using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedSnmpSensor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EndIndex",
                table: "SnmpSensors",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMulti",
                table: "SnmpSensors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StartIndex",
                table: "SnmpSensors",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndIndex",
                table: "SnmpSensors");

            migrationBuilder.DropColumn(
                name: "IsMulti",
                table: "SnmpSensors");

            migrationBuilder.DropColumn(
                name: "StartIndex",
                table: "SnmpSensors");
        }
    }
}

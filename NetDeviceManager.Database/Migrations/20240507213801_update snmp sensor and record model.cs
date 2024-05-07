using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class updatesnmpsensorandrecordmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "SnmpSensorRecords");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "SnmpSensorRecords",
                newName: "Data");

            migrationBuilder.AlterColumn<int>(
                name: "StartIndex",
                table: "SnmpSensors",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EndIndex",
                table: "SnmpSensors",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "SnmpSensorRecords",
                newName: "Value");

            migrationBuilder.AlterColumn<int>(
                name: "StartIndex",
                table: "SnmpSensors",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "EndIndex",
                table: "SnmpSensors",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "SnmpSensorRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class Updatesnmprecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "SnmpSensorRecords");

            migrationBuilder.AddColumn<int>(
                name: "Index",
                table: "SnmpSensorRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "SnmpSensorRecords");

            migrationBuilder.AddColumn<string>(
                name: "ItemId",
                table: "SnmpSensorRecords",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class renamepropinsnmprecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnmpSensorRecords_SensorsInPhysicalDevices_SensorInPhysical~",
                table: "SnmpSensorRecords");

            migrationBuilder.DropIndex(
                name: "IX_SnmpSensorRecords_SensorInPhysicalPhysicalDeviceId",
                table: "SnmpSensorRecords");

            migrationBuilder.DropColumn(
                name: "SensorInPhysicalPhysicalDeviceId",
                table: "SnmpSensorRecords");

            migrationBuilder.CreateIndex(
                name: "IX_SnmpSensorRecords_SensorInPhysicalDeviceId",
                table: "SnmpSensorRecords",
                column: "SensorInPhysicalDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnmpSensorRecords_SensorsInPhysicalDevices_SensorInPhysical~",
                table: "SnmpSensorRecords",
                column: "SensorInPhysicalDeviceId",
                principalTable: "SensorsInPhysicalDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnmpSensorRecords_SensorsInPhysicalDevices_SensorInPhysical~",
                table: "SnmpSensorRecords");

            migrationBuilder.DropIndex(
                name: "IX_SnmpSensorRecords_SensorInPhysicalDeviceId",
                table: "SnmpSensorRecords");

            migrationBuilder.AddColumn<Guid>(
                name: "SensorInPhysicalPhysicalDeviceId",
                table: "SnmpSensorRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SnmpSensorRecords_SensorInPhysicalPhysicalDeviceId",
                table: "SnmpSensorRecords",
                column: "SensorInPhysicalPhysicalDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnmpSensorRecords_SensorsInPhysicalDevices_SensorInPhysical~",
                table: "SnmpSensorRecords",
                column: "SensorInPhysicalPhysicalDeviceId",
                principalTable: "SensorsInPhysicalDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

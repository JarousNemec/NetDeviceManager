using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class changedstructureofsnmprecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnmpSensorRecords_SnmpSensorsInPhysicalDevices_SensorInPhys~",
                table: "SnmpSensorRecords");

            migrationBuilder.RenameColumn(
                name: "SensorInPhysicalDeviceId",
                table: "SnmpSensorRecords",
                newName: "SensorId");

            migrationBuilder.RenameIndex(
                name: "IX_SnmpSensorRecords_SensorInPhysicalDeviceId",
                table: "SnmpSensorRecords",
                newName: "IX_SnmpSensorRecords_SensorId");

            migrationBuilder.AddColumn<Guid>(
                name: "PhysicalDeviceId",
                table: "SnmpSensorRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_SnmpSensorRecords_PhysicalDeviceId",
                table: "SnmpSensorRecords",
                column: "PhysicalDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnmpSensorRecords_PhysicalDevices_PhysicalDeviceId",
                table: "SnmpSensorRecords",
                column: "PhysicalDeviceId",
                principalTable: "PhysicalDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SnmpSensorRecords_SnmpSensors_SensorId",
                table: "SnmpSensorRecords",
                column: "SensorId",
                principalTable: "SnmpSensors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SnmpSensorRecords_PhysicalDevices_PhysicalDeviceId",
                table: "SnmpSensorRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_SnmpSensorRecords_SnmpSensors_SensorId",
                table: "SnmpSensorRecords");

            migrationBuilder.DropIndex(
                name: "IX_SnmpSensorRecords_PhysicalDeviceId",
                table: "SnmpSensorRecords");

            migrationBuilder.DropColumn(
                name: "PhysicalDeviceId",
                table: "SnmpSensorRecords");

            migrationBuilder.RenameColumn(
                name: "SensorId",
                table: "SnmpSensorRecords",
                newName: "SensorInPhysicalDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_SnmpSensorRecords_SensorId",
                table: "SnmpSensorRecords",
                newName: "IX_SnmpSensorRecords_SensorInPhysicalDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_SnmpSensorRecords_SnmpSensorsInPhysicalDevices_SensorInPhys~",
                table: "SnmpSensorRecords",
                column: "SensorInPhysicalDeviceId",
                principalTable: "SnmpSensorsInPhysicalDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

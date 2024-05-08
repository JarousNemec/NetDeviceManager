using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class addednewtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OidFilling",
                table: "SnmpSensors",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CorrectDataPatterns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<string>(type: "text", nullable: false),
                    CapturedTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PhysicalDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SensorId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorrectDataPatterns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorrectDataPatterns_PhysicalDevices_PhysicalDeviceId",
                        column: x => x.PhysicalDeviceId,
                        principalTable: "PhysicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CorrectDataPatterns_SnmpSensors_SensorId",
                        column: x => x.SensorId,
                        principalTable: "SnmpSensors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CorrectDataPatterns_PhysicalDeviceId",
                table: "CorrectDataPatterns",
                column: "PhysicalDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_CorrectDataPatterns_SensorId",
                table: "CorrectDataPatterns",
                column: "SensorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorrectDataPatterns");

            migrationBuilder.DropColumn(
                name: "OidFilling",
                table: "SnmpSensors");
        }
    }
}

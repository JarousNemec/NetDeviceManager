using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class TableRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhysicalDevicesReadIntervals");

            migrationBuilder.CreateTable(
                name: "PhysicalDevicesReadJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SchedulerCron = table.Column<string>(type: "text", nullable: false),
                    PhysicalDeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalDevicesReadJobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalDevicesReadJobs_PhysicalDevices_PhysicalDeviceId",
                        column: x => x.PhysicalDeviceId,
                        principalTable: "PhysicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalDevicesReadJobs_PhysicalDeviceId",
                table: "PhysicalDevicesReadJobs",
                column: "PhysicalDeviceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhysicalDevicesReadJobs");

            migrationBuilder.CreateTable(
                name: "PhysicalDevicesReadIntervals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhysicalDeviceId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchedulerCron = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalDevicesReadIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalDevicesReadIntervals_PhysicalDevices_PhysicalDevice~",
                        column: x => x.PhysicalDeviceId,
                        principalTable: "PhysicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalDevicesReadIntervals_PhysicalDeviceId",
                table: "PhysicalDevicesReadIntervals",
                column: "PhysicalDeviceId");
        }
    }
}

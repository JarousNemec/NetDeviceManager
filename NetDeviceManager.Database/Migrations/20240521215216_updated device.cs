using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class updateddevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceIcons_IconId",
                table: "Devices");

            migrationBuilder.AlterColumn<Guid>(
                name: "IconId",
                table: "Devices",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceIcons_IconId",
                table: "Devices",
                column: "IconId",
                principalTable: "DeviceIcons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_DeviceIcons_IconId",
                table: "Devices");

            migrationBuilder.AlterColumn<Guid>(
                name: "IconId",
                table: "Devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_DeviceIcons_IconId",
                table: "Devices",
                column: "IconId",
                principalTable: "DeviceIcons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

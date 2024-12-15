using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDeviceAndUpdatedConcept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevices_Devices_DeviceId",
                table: "PhysicalDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevices_LoginProfiles_LoginProfileId",
                table: "PhysicalDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevicesHasPorts_PhysicalDevices_DeviceId",
                table: "PhysicalDevicesHasPorts");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevicesHasPorts_Ports_PortId",
                table: "PhysicalDevicesHasPorts");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_PhysicalDevices_DeviceId",
                table: "PhysicalDevices");

            migrationBuilder.DropIndex(
                name: "IX_PhysicalDevices_LoginProfileId",
                table: "PhysicalDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhysicalDevicesHasPorts",
                table: "PhysicalDevicesHasPorts");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "PhysicalDevices");

            migrationBuilder.DropColumn(
                name: "LoginProfileId",
                table: "PhysicalDevices");

            migrationBuilder.RenameTable(
                name: "PhysicalDevicesHasPorts",
                newName: "PhysicalDevicesHavePorts");

            migrationBuilder.RenameColumn(
                name: "IpAddress",
                table: "PhysicalDevices",
                newName: "Platform");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "LoginProfiles",
                newName: "SshUsername");

            migrationBuilder.RenameColumn(
                name: "SecurityName",
                table: "LoginProfiles",
                newName: "SshPassword");

            migrationBuilder.RenameColumn(
                name: "PrivacyPassword",
                table: "LoginProfiles",
                newName: "SnmpUsername");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "LoginProfiles",
                newName: "SnmpSecurityName");

            migrationBuilder.RenameColumn(
                name: "Key",
                table: "LoginProfiles",
                newName: "SnmpPrivacyPassword");

            migrationBuilder.RenameColumn(
                name: "ConnString",
                table: "LoginProfiles",
                newName: "SnmpAuthenticationPassword");

            migrationBuilder.RenameColumn(
                name: "AuthenticationPassword",
                table: "LoginProfiles",
                newName: "CiscoPrivilagedModePassword");

            migrationBuilder.RenameIndex(
                name: "IX_PhysicalDevicesHasPorts_PortId",
                table: "PhysicalDevicesHavePorts",
                newName: "IX_PhysicalDevicesHavePorts_PortId");

            migrationBuilder.RenameIndex(
                name: "IX_PhysicalDevicesHasPorts_DeviceId",
                table: "PhysicalDevicesHavePorts",
                newName: "IX_PhysicalDevicesHavePorts_DeviceId");

            migrationBuilder.AddColumn<string>(
                name: "Capabilities",
                table: "PhysicalDevices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "IconId",
                table: "PhysicalDevices",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "PhysicalDevices",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "DeviceIcons",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhysicalDevicesHavePorts",
                table: "PhysicalDevicesHavePorts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "LoginProfilesToPhysicalDevices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    PhysicalDeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginProfilesToPhysicalDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginProfilesToPhysicalDevices_LoginProfiles_LoginProfileId",
                        column: x => x.LoginProfileId,
                        principalTable: "LoginProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoginProfilesToPhysicalDevices_PhysicalDevices_PhysicalDevi~",
                        column: x => x.PhysicalDeviceId,
                        principalTable: "PhysicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalDevicesHaveIpAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "text", nullable: false),
                    PhysicalDeviceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalDevicesHaveIpAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PhysicalDevicesHaveIpAddresses_PhysicalDevices_PhysicalDevi~",
                        column: x => x.PhysicalDeviceId,
                        principalTable: "PhysicalDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalDevices_IconId",
                table: "PhysicalDevices",
                column: "IconId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginProfilesToPhysicalDevices_LoginProfileId",
                table: "LoginProfilesToPhysicalDevices",
                column: "LoginProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginProfilesToPhysicalDevices_PhysicalDeviceId",
                table: "LoginProfilesToPhysicalDevices",
                column: "PhysicalDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalDevicesHaveIpAddresses_PhysicalDeviceId",
                table: "PhysicalDevicesHaveIpAddresses",
                column: "PhysicalDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevices_DeviceIcons_IconId",
                table: "PhysicalDevices",
                column: "IconId",
                principalTable: "DeviceIcons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevicesHavePorts_PhysicalDevices_DeviceId",
                table: "PhysicalDevicesHavePorts",
                column: "DeviceId",
                principalTable: "PhysicalDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevicesHavePorts_Ports_PortId",
                table: "PhysicalDevicesHavePorts",
                column: "PortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevices_DeviceIcons_IconId",
                table: "PhysicalDevices");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevicesHavePorts_PhysicalDevices_DeviceId",
                table: "PhysicalDevicesHavePorts");

            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevicesHavePorts_Ports_PortId",
                table: "PhysicalDevicesHavePorts");

            migrationBuilder.DropTable(
                name: "LoginProfilesToPhysicalDevices");

            migrationBuilder.DropTable(
                name: "PhysicalDevicesHaveIpAddresses");

            migrationBuilder.DropIndex(
                name: "IX_PhysicalDevices_IconId",
                table: "PhysicalDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhysicalDevicesHavePorts",
                table: "PhysicalDevicesHavePorts");

            migrationBuilder.DropColumn(
                name: "Capabilities",
                table: "PhysicalDevices");

            migrationBuilder.DropColumn(
                name: "IconId",
                table: "PhysicalDevices");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "PhysicalDevices");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "DeviceIcons");

            migrationBuilder.RenameTable(
                name: "PhysicalDevicesHavePorts",
                newName: "PhysicalDevicesHasPorts");

            migrationBuilder.RenameColumn(
                name: "Platform",
                table: "PhysicalDevices",
                newName: "IpAddress");

            migrationBuilder.RenameColumn(
                name: "SshUsername",
                table: "LoginProfiles",
                newName: "Username");

            migrationBuilder.RenameColumn(
                name: "SshPassword",
                table: "LoginProfiles",
                newName: "SecurityName");

            migrationBuilder.RenameColumn(
                name: "SnmpUsername",
                table: "LoginProfiles",
                newName: "PrivacyPassword");

            migrationBuilder.RenameColumn(
                name: "SnmpSecurityName",
                table: "LoginProfiles",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "SnmpPrivacyPassword",
                table: "LoginProfiles",
                newName: "Key");

            migrationBuilder.RenameColumn(
                name: "SnmpAuthenticationPassword",
                table: "LoginProfiles",
                newName: "ConnString");

            migrationBuilder.RenameColumn(
                name: "CiscoPrivilagedModePassword",
                table: "LoginProfiles",
                newName: "AuthenticationPassword");

            migrationBuilder.RenameIndex(
                name: "IX_PhysicalDevicesHavePorts_PortId",
                table: "PhysicalDevicesHasPorts",
                newName: "IX_PhysicalDevicesHasPorts_PortId");

            migrationBuilder.RenameIndex(
                name: "IX_PhysicalDevicesHavePorts_DeviceId",
                table: "PhysicalDevicesHasPorts",
                newName: "IX_PhysicalDevicesHasPorts_DeviceId");

            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "PhysicalDevices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LoginProfileId",
                table: "PhysicalDevices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhysicalDevicesHasPorts",
                table: "PhysicalDevicesHasPorts",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IconId = table.Column<Guid>(type: "uuid", nullable: true),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_DeviceIcons_IconId",
                        column: x => x.IconId,
                        principalTable: "DeviceIcons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalDevices_DeviceId",
                table: "PhysicalDevices",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalDevices_LoginProfileId",
                table: "PhysicalDevices",
                column: "LoginProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_IconId",
                table: "Devices",
                column: "IconId");

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevices_Devices_DeviceId",
                table: "PhysicalDevices",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevices_LoginProfiles_LoginProfileId",
                table: "PhysicalDevices",
                column: "LoginProfileId",
                principalTable: "LoginProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevicesHasPorts_PhysicalDevices_DeviceId",
                table: "PhysicalDevicesHasPorts",
                column: "DeviceId",
                principalTable: "PhysicalDevices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevicesHasPorts_Ports_PortId",
                table: "PhysicalDevicesHasPorts",
                column: "PortId",
                principalTable: "Ports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

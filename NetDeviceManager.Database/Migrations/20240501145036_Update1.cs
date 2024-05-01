using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevices_CredentialsDatas_LoginProfileId",
                table: "PhysicalDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CredentialsDatas",
                table: "CredentialsDatas");

            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "SnmpSensors");

            migrationBuilder.RenameTable(
                name: "CredentialsDatas",
                newName: "LoginProfiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LoginProfiles",
                table: "LoginProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevices_LoginProfiles_LoginProfileId",
                table: "PhysicalDevices",
                column: "LoginProfileId",
                principalTable: "LoginProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhysicalDevices_LoginProfiles_LoginProfileId",
                table: "PhysicalDevices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LoginProfiles",
                table: "LoginProfiles");

            migrationBuilder.RenameTable(
                name: "LoginProfiles",
                newName: "CredentialsDatas");

            migrationBuilder.AddColumn<Guid>(
                name: "CommunityId",
                table: "SnmpSensors",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CredentialsDatas",
                table: "CredentialsDatas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PhysicalDevices_CredentialsDatas_LoginProfileId",
                table: "PhysicalDevices",
                column: "LoginProfileId",
                principalTable: "CredentialsDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

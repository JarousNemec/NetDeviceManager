using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class updatesyslog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Application",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "Hostname",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SyslogRecords");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "SyslogRecords",
                newName: "CompletMessage");

            migrationBuilder.AddColumn<int>(
                name: "Facility",
                table: "SyslogRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedDate",
                table: "SyslogRecords",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "SyslogRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Facility",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "ProcessedDate",
                table: "SyslogRecords");

            migrationBuilder.DropColumn(
                name: "Severity",
                table: "SyslogRecords");

            migrationBuilder.RenameColumn(
                name: "CompletMessage",
                table: "SyslogRecords",
                newName: "Priority");

            migrationBuilder.AddColumn<string>(
                name: "Application",
                table: "SyslogRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hostname",
                table: "SyslogRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "SyslogRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessId",
                table: "SyslogRecords",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Timestamp",
                table: "SyslogRecords",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "SyslogRecords",
                type: "text",
                nullable: true);
        }
    }
}

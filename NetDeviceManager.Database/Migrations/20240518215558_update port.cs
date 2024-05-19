using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class updateport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Ports");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Ports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Ports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetDeviceManager.Database.Migrations
{
    /// <inheritdoc />
    public partial class updatedport2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Ports",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Ports");
        }
    }
}

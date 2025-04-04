using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddWaterType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "HandiProcesses");

            migrationBuilder.AddColumn<string>(
                name: "WaterType",
                table: "HandiProcesses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterType",
                table: "HandiProcesses");

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "HandiProcesses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}

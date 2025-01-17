using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberofhandi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HandiRunCount",
                table: "HandiProcesses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnloadBunkerName",
                table: "HandiProcesses",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandiRunCount",
                table: "HandiProcesses");

            migrationBuilder.DropColumn(
                name: "UnloadBunkerName",
                table: "HandiProcesses");
        }
    }
}

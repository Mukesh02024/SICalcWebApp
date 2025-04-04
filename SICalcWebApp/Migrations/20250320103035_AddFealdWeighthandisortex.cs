using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFealdWeighthandisortex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandiRunCount",
                table: "HandiProcesses");

            migrationBuilder.AddColumn<decimal>(
                name: "EndWeight",
                table: "SortexProcesses",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PaddyWeight",
                table: "HandiProcesses",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndWeight",
                table: "SortexProcesses");

            migrationBuilder.DropColumn(
                name: "PaddyWeight",
                table: "HandiProcesses");

            migrationBuilder.AddColumn<int>(
                name: "HandiRunCount",
                table: "HandiProcesses",
                type: "int",
                nullable: true);
        }
    }
}

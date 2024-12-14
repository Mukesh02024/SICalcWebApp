using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddtableDryer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DryerProcesses",
                columns: table => new
                {
                    DryerProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuctiPressure = table.Column<double>(type: "float", nullable: false),
                    UnloadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnloadBunkerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PauseReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PauseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResumeTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalDelayTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DryerProcesses", x => x.DryerProcessId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DryerProcesses");
        }
    }
}

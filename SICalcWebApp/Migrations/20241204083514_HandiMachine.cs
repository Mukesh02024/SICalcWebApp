using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class HandiMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.CreateTable(
                name: "HandiProcesses",
                columns: table => new
                {
                    HandiProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaddyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandiType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Pressure = table.Column<double>(type: "float", nullable: false),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ProcessStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PauseReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandiProcesses", x => x.HandiProcessId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HandiProcesses");

            migrationBuilder.CreateTable(
                name: "MachineProcesses",
                columns: table => new
                {
                    MachineProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuctPressure = table.Column<int>(type: "int", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HandiType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MachineName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaddyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PauseReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PauseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Pressure = table.Column<int>(type: "int", nullable: true),
                    ProcessStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResumeTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Temperature = table.Column<int>(type: "int", nullable: true),
                    UnloadBunkerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineProcesses", x => x.MachineProcessId);
                });
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTableforMillMachine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MillingProcesses",
                columns: table => new
                {
                    MillingProcessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MillBunkerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexBunkerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StaffName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PauseReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PauseTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResumeTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalDelayTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MillingProcesses", x => x.MillingProcessId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MillingProcesses");
        }
    }
}

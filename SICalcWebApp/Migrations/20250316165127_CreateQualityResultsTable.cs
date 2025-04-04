using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateQualityResultsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BatchProcessReportArwaVMs",
                columns: table => new
                {
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaddyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandiType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pressure = table.Column<double>(type: "float", nullable: false),
                    HandiStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandiStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandiEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandiDelay = table.Column<double>(type: "float", nullable: false),
                    HandiTakenTime = table.Column<double>(type: "float", nullable: false),
                    MillStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MillEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MillBunkerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexUnloadBunk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MillingStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MillingDelay = table.Column<double>(type: "float", nullable: false),
                    MillingTakenTime = table.Column<double>(type: "float", nullable: false),
                    SortexStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortexEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortexloadBunk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexDelay = table.Column<double>(type: "float", nullable: false),
                    SortexTakenTime = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "BatchProcessReports",
                columns: table => new
                {
                    BatchId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProcessType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaddyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandiType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pressure = table.Column<double>(type: "float", nullable: false),
                    HandiStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HandiStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandiEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HandiDelay = table.Column<double>(type: "float", nullable: false),
                    HandiTakenTime = table.Column<double>(type: "float", nullable: false),
                    DryerHandiTimeDifference = table.Column<double>(type: "float", nullable: false),
                    DryerLoadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DryerUnloadTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuctiPressure = table.Column<double>(type: "float", nullable: false),
                    DryerUnloadBunk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DryerStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DryerDelay = table.Column<double>(type: "float", nullable: true),
                    DryerTakenTime = table.Column<double>(type: "float", nullable: false),
                    MillStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MillEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MillBunkerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexUnloadBunk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MillingStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MillingDelay = table.Column<double>(type: "float", nullable: false),
                    MillingTakenTime = table.Column<double>(type: "float", nullable: false),
                    SortexStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortexEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SortexloadBunk = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexStaff = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SortexDelay = table.Column<double>(type: "float", nullable: false),
                    SortexTakenTime = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "MillQualities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Machine_Damage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Manual_Damage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Machine_Discolour = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Manual_Discolour = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Machine_Broken = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Manual_Broken = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Machine_FRK = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Manual_FRK = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Machine_Moisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Manual_Moisture = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Moisture_Chotti_Machine = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Moisture_Chotti_Machine_Manual = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Mill_Wightment = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Other1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MillQualities", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MillQualities_BatchID_Stage",
                table: "MillQualities",
                columns: new[] { "BatchID", "Stage" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BatchProcessReportArwaVMs");

            migrationBuilder.DropTable(
                name: "BatchProcessReports");

            migrationBuilder.DropTable(
                name: "MillQualities");
        }
    }
}

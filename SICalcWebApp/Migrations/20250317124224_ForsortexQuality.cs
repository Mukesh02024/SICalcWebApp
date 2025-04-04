using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ForsortexQuality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MillQualitySortexes",
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
                    Sec30Final = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Min10Rejection = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ReportedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Other1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Other3 = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MillQualitySortexes", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MillQualitySortexes_BatchID_Stage",
                table: "MillQualitySortexes",
                columns: new[] { "BatchID", "Stage" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MillQualitySortexes");
        }
    }
}

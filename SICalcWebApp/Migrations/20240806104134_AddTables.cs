using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    


            migrationBuilder.CreateTable(
                name: "FCs",
                columns: table => new
                {
                    FCId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FCValue = table.Column<int>(type: "int", nullable: false),
                    C_Fe = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FCs", x => x.FCId);
                });

            migrationBuilder.CreateTable(
                name: "TPDInfos",
                columns: table => new
                {
                    TPDId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TPD = table.Column<int>(type: "int", nullable: false),
                    Kiln = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPDInfos", x => x.TPDId);
                });

            migrationBuilder.CreateTable(
                name: "FCInfos",
                columns: table => new
                {
                    FCInfoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FCId = table.Column<int>(type: "int", nullable: false),
                    TPDId = table.Column<int>(type: "int", nullable: false),
                    FeedRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FCInfos", x => x.FCInfoId);
                    table.ForeignKey(
                        name: "FK_FCInfos_FCs_FCId",
                        column: x => x.FCId,
                        principalTable: "FCs",
                        principalColumn: "FCId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FCInfos_TPDInfos_TPDId",
                        column: x => x.TPDId,
                        principalTable: "TPDInfos",
                        principalColumn: "TPDId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FCInfos_FCId",
                table: "FCInfos",
                column: "FCId");

            migrationBuilder.CreateIndex(
                name: "IX_FCInfos_TPDId",
                table: "FCInfos",
                column: "TPDId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FCInfos");

            migrationBuilder.DropTable(
                name: "FCs");

            migrationBuilder.DropTable(
                name: "TPDInfos");
        }
    }
}

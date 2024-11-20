using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SICalcWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddIronType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IronTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IronTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IronTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InputOperands",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sidename = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IronTypeId = table.Column<int>(type: "int", nullable: false),
                    FeT = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Moisture = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Gangue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Phos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Yield = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IronPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GroundLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FineLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinesRealisation = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FinesRealisationValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FeMSponge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransferLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    YLoss = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InputOperands", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_InputOperands_IronTypes_IronTypeId",
                        column: x => x.IronTypeId,
                        principalTable: "IronTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InputOperands_IronTypeId",
                table: "InputOperands",
                column: "IronTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InputOperands");

            migrationBuilder.DropTable(
                name: "IronTypes");
        }
    }
}

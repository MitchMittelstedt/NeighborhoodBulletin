using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Barcodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    value = table.Column<string>(nullable: true),
                    NeighborId = table.Column<int>(nullable: false),
                    ShopOwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barcodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Barcodes_Neighbors_NeighborId",
                        column: x => x.NeighborId,
                        principalTable: "Neighbors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Barcodes_ShopOwners_ShopOwnerId",
                        column: x => x.ShopOwnerId,
                        principalTable: "ShopOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_NeighborId",
                table: "Barcodes",
                column: "NeighborId");

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_ShopOwnerId",
                table: "Barcodes",
                column: "ShopOwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Barcodes");
        }
    }
}

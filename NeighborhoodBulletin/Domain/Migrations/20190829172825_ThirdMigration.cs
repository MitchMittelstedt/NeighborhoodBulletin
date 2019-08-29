using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class ThirdMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Barcodes_Neighbors_NeighborId",
                table: "Barcodes");

            migrationBuilder.DropIndex(
                name: "IX_Barcodes_NeighborId",
                table: "Barcodes");

            migrationBuilder.DropColumn(
                name: "NeighborId",
                table: "Barcodes");

            migrationBuilder.AddColumn<int>(
                name: "BarcodeCount",
                table: "Updates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BarcodeCount",
                table: "Updates");

            migrationBuilder.AddColumn<int>(
                name: "NeighborId",
                table: "Barcodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Barcodes_NeighborId",
                table: "Barcodes",
                column: "NeighborId");

            migrationBuilder.AddForeignKey(
                name: "FK_Barcodes_Neighbors_NeighborId",
                table: "Barcodes",
                column: "NeighborId",
                principalTable: "Neighbors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

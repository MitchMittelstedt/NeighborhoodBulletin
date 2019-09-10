using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class FourthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SingleUseCoupons_ShopOwners_ShopOwnerId",
                table: "SingleUseCoupons");

            migrationBuilder.DropIndex(
                name: "IX_SingleUseCoupons_ShopOwnerId",
                table: "SingleUseCoupons");

            migrationBuilder.DropColumn(
                name: "ShopOwnerId",
                table: "SingleUseCoupons");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopOwnerId",
                table: "SingleUseCoupons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SingleUseCoupons_ShopOwnerId",
                table: "SingleUseCoupons",
                column: "ShopOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SingleUseCoupons_ShopOwners_ShopOwnerId",
                table: "SingleUseCoupons",
                column: "ShopOwnerId",
                principalTable: "ShopOwners",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutsideShopOwnerZipCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NonlocalZipCode = table.Column<int>(nullable: false),
                    ShopOwnerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutsideShopOwnerZipCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutsideShopOwnerZipCodes_ShopOwners_ShopOwnerId",
                        column: x => x.ShopOwnerId,
                        principalTable: "ShopOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutsideShopOwnerZipCodes_ShopOwnerId",
                table: "OutsideShopOwnerZipCodes",
                column: "ShopOwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutsideShopOwnerZipCodes");
        }
    }
}

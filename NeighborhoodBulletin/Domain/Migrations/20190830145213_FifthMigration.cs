using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class FifthMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopOwnerZipCode",
                table: "Updates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShopOwnerZipCode",
                table: "Updates");
        }
    }
}

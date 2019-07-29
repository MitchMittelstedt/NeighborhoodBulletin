using Microsoft.EntityFrameworkCore.Migrations;

namespace Domain.Migrations
{
    public partial class SecondMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Updates",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasBarcode",
                table: "Updates",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Updates");

            migrationBuilder.DropColumn(
                name: "HasBarcode",
                table: "Updates");
        }
    }
}
